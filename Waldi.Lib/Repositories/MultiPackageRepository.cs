using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Waldi.Engine;
using Waldi.Packages;

namespace Waldi.Repositories
{
    public class MultiPackageRepository : IPackageRepository, IEquatable<MultiPackageRepository>
	{
        public List<IPackageRepository> Repositories { get; private set; }
        public string Name { get; private set; }

        public MultiPackageRepository (string name, params IPackageRepository[] repositories)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException ("Name cannot be null or empty.","name");
            }
            this.Name = name;
            this.Repositories = new List<IPackageRepository> (repositories);
        }

        public MultiPackageRepository (string name, IEnumerable<IPackageRepository> repositories)
		{
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException ("Name cannot be null or empty.","name");
            }
            this.Name = name;
            this.Repositories = new List<IPackageRepository> (repositories);
		}

		public MultiPackageRepository (string name)
		{
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException ("Name cannot be null or empty.","name");
            }
            this.Name = name;
            this.Repositories = new List<IPackageRepository>();
		}

		public void Refresh()
		{
            foreach (IPackageRepository rep in this.Repositories)
			{
				rep.Refresh ();
			}
		}

		public PackageList GetPackages()
		{
			PackageList plist = new PackageList ();
            foreach (IPackageRepository rep in this.Repositories)
			{
				plist.AddRange (rep.GetPackages ());
			}
			return plist;
		}

		public IPackage GetPackage(string name)
		{
            IPackageRepository rep;
            IPackage pkg = this.GetPackage(name, out rep);
            return pkg;
		}

        private IPackage GetPackage(string name, out IPackageRepository repository)
        {
            foreach (IPackageRepository rep in this.Repositories)
            {
                IPackage p = rep.GetPackage (name);
                if (p != null)
                {
                    repository = rep;
                    return p;
                }
            }
            repository = null;
            return null;
        }

		public void CopyPackageFiles(string pkgname, string pathtodestdir)
        {
            IPackageRepository rep;
            IPackage pkg = this.GetPackage(pkgname, out rep);
            if (pkg != null)
            {
                rep.CopyPackageFiles(pkgname, pathtodestdir);
            }
            else
            {
                throw new ArgumentException ("A package named " + pkgname + " does not exist.", "pkgname");
            }
        }

        public void AddPackage(IPackage pkg, string pathtosourcedir)
        {
            throw new NotImplementedException();
        }

        public bool Equals (MultiPackageRepository other)
        {
            return this.Equals ((IPackageRepository)other);
        }

        public static bool Equals (IPackageRepository a, IPackageRepository b)
        {
            if (a == b)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }
            return a.Equals(b);
        }

        public bool Equals (IPackageRepository other)
        {           
            if (this == other)
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            if (!(other is MultiPackageRepository))
            {
                return false;
            }
            MultiPackageRepository othertyped = other as MultiPackageRepository;
            bool areequal = string.Equals (this.Name, othertyped.Name);
            areequal = areequal && Enumerable.SequenceEqual<IPackageRepository>(this.Repositories, othertyped.Repositories);             
            return areequal;
        }
	}
}