using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldi.Packages;

namespace Waldi.Engine
{
    public class DependencyResolver
    {
		public static List<IPackage> OrderByDependencies(PackageList packages)
		{
			List<IPackage> orderedpackages = new List<IPackage>();
			foreach (IPackage pack in packages)
			{
				if (orderedpackages.Contains(pack))
				{
					continue;
				}
				List<IPackage> deps = GetDependencies (packages, pack);
				foreach (IPackage p in deps)
				{
					if (!orderedpackages.Contains (p))
					{
						orderedpackages.Add (p);
					}
				}
				if (!orderedpackages.Contains(pack))
				{
					orderedpackages.Add (pack);
				}
			}
			return orderedpackages;
		}

		public static void EnableFeatures(PackageList packages, bool includeoptional = false)
		{
			foreach (IPackage p in packages)
			{
				foreach(Dependency d in p.Dependencies)
				{
					if (d.IsOptional && !includeoptional) {
						continue;
					}
					IPackage depp = FindPackage(packages, d);
					if (depp == null)
					{
						throw new InvalidDependencyException("Could not find dependency '" + d.Name + "'",p , null);
					}
					if (!string.IsNullOrEmpty (d.FeatureName))
					{
						//depp.Config.EnableFeature (d.FeatureName);
//						IPackage pack = depp.Packages.FindByName (d.PackageName);
//						if (pack == null)
//						{
//							throw new Exception ("Package not found in packages.");
//						}
						Feature feat = depp.Features.FindByName (d.FeatureName);
						if (feat == null)
						{
							throw new Exception ("Feature not found in features.");
						}
						feat.Enabled = true;
					}
				}
			}
		}

		public static void AddDependencePackages(PackageList packages)
		{
			foreach (IPackage pack in packages)
			{
				List<IPackage> deps = GetDependencies (packages, pack, false, true);
				pack.Packages.AddRange (deps);
			}
		}

        public static void SelectVersions(PackageList packages)
        {
            Dictionary<string,List<PackageVersion>> neededversions = new Dictionary<string, List<PackageVersion>>();
            foreach (IPackage pack in packages)
            {
                foreach (Dependency dep in pack.Dependencies)
                {
                    if (!neededversions.ContainsKey(dep.PackageName))
                    {
                        neededversions.Add(dep.PackageName, new List<PackageVersion>());
                    }
                    neededversions[dep.PackageName].Add(dep.Version);
                }
            }

            foreach (string pkgname in neededversions.Keys)
            {
                IPackage pkg = packages[pkgname];
                try
                {
                    PackageVersion pkgversion = GetHighestMatchingVersion(pkg.Versions, neededversions[pkgname]);
                    pkg.SelectedVersion = pkgversion;
                }
                catch(InvalidDependencyException ex)
                {
                    throw new InvalidDependencyException("No Version of Package " + pkg.Name + " found which satisfies all Dependencies.", ex, pkg);
                }
            }
        }


        private static PackageVersion GetHighestMatchingVersion(IEnumerable<PackageVersion> versionpool, IEnumerable<PackageVersion> tomatch)
        {
            // vielleicht die Methode in PackageVersionList?
            List<PackageVersion> sortedversion = new List<PackageVersion>(versionpool);
            sortedversion.Sort();
            PackageVersion notmatchingversion = null;
            foreach (PackageVersion pv in sortedversion)
            {
                bool allmatch = true;
                foreach(PackageVersion m in tomatch)
                {
                    allmatch = allmatch && m.Matches(pv);
                    if (!allmatch)
                    {
                        notmatchingversion = m;
                        break;
                    }
                }
                if (allmatch)
                {
                    return pv;
                }
            }
            throw new InvalidDependencyException("No Version found to satisfy Version " + notmatchingversion.Name + ".");
        }

		private static List<IPackage> GetDependencies(PackageList packages, IPackage pack, bool recursive = true, bool unique = false)
		{
			List<IPackage> deps = new List<IPackage> ();
			foreach (Dependency dep in pack.Dependencies)
			{	
				IPackage deppack = FindPackage(packages, dep);
				if (deppack == null)
				{
					throw new InvalidDependencyException("Could not find dependency '" + dep.Name + "'", pack, null);
				}
				if (IsCircularDependency(dep,deppack))
				{
					throw new InvalidDependencyException("Circular dependency detected.", pack, deppack);
				}
				if (recursive)
				{
					deps.AddRange(GetDependencies(packages, deppack, recursive, unique));
				}
				if (!unique || (unique && !deps.Contains(deppack)))
				{
					deps.Add (deppack);
				}
			}
			return deps;
		}

		private static bool IsCircularDependency(Dependency dep, IPackage deppack)
		{
			IEnumerable<Dependency> circdeps = deppack.Dependencies.Where (d => d.PackageName == dep.PackageName);
			return circdeps.Count() > 0;
		}

		private static IPackage FindPackage(PackageList packages, Dependency dep)
		{
			IPackage pack = packages.FindByName (dep.PackageName);
			if (pack == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty (dep.FeatureName)) {
				Feature feature = pack.Features.Where (f => f.Name == dep.FeatureName).FirstOrDefault ();
				if (feature == null) {
					return null;
				}
			}
			return pack;
		}
    }
}
