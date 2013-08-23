using System;
using System.IO;
using Waldi.Engine;
using Waldi.Packages;
using Waldi.Serialization;
using Waldi.BclExtensions;
using Waldi.Lib;

namespace Waldi.Repositories
{
    public class DirectoryPackageRepository : IPackageRepository, IEquatable<DirectoryPackageRepository>
	{
		public string Name { get; private set;}
		public DirectoryInfo PackageDir { get; private set;}
		private PackageList packages;
        private bool allpackagesread = false;

		public DirectoryPackageRepository (string name, string pathtopackagedir) : this(name, new DirectoryInfo(pathtopackagedir))
		{
		}

		public DirectoryPackageRepository (string name, DirectoryInfo packagedir)
		{
			if (packagedir == null)
			{
				throw new ArgumentNullException ("packagedir");
			}
			if (!packagedir.Exists)
			{
				throw new ArgumentException ("Package dir does not exist.", "packagedir");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException ("Name cannot be null or empty.","name");
			}
			this.PackageDir = packagedir;
			this.Name = name;
		}

		public void Refresh()
		{
			this.packages = null;
		}

		public PackageList GetPackages()
		{
            if (this.packages == null || !this.allpackagesread)
			{
				this.packages = this.ReadRepositoryDir(this.PackageDir);
                this.allpackagesread = true;
			}
			return this.packages;
		}

		public IPackage GetPackage(string pkgname)
		{
			// only read needed package, not all (if not already done)
			if (this.packages != null)
            {
                IPackage pkg = this.packages.FindByName(pkgname);
                if (pkg != null || this.allpackagesread)
                {
                    return pkg;
                }
            }
            DirectoryInfo pkgdir = new DirectoryInfo(Path.Combine(this.PackageDir.FullName, pkgname));
            if (!pkgdir.Exists)
            {
                return null;
            }
            IPackage newpkg = this.ReadPackageDir(pkgdir);
            if (newpkg == null)
            {
                return null;
            }
            if (this.packages == null)
            {
                this.packages = new PackageList();
            }
            this.packages.Add(newpkg);
            return newpkg;
        }

		public void CopyPackageFiles(string pkgname, string pathtodestdir)
        {
			IPackage pkg = this.GetPackage (pkgname);
			if (pkg == null)
			{
				throw new ArgumentException ("A package named " + pkgname + " does not exist.", "pkgname");
			}
			string pkgdirpath = Path.Combine (this.PackageDir.FullName, pkgname);
			DirectoryInfo pkgdir = new DirectoryInfo (pkgdirpath);
			if (!pkgdir.Exists)
			{
                throw new PackageIoException ("Package directory of package " + pkgname + " does not exist: " + pkgdir.FullName, pkgname);
			}
			DirectoryInfo destdir = new DirectoryInfo (pathtodestdir);
			if (!destdir.IsEmpty ())
			{
                throw new PackageIoException ("Package destination directory is not empty: " + pathtodestdir);
			}
            pkgdir.CopyTo (destdir.FullName, true);
        }

        public void AddPackage(IPackage pkg, string pathtosourcedir)
        {
            if (pkg == null)
            {
                throw new ArgumentNullException("pkg");
            }
            if (this.GetPackage (pkg.Name) != null)
            {
                throw new ArgumentException ("A package named " + pkg.Name + " already exists.", "pkgname");
            }
            DirectoryInfo sourcedir = new DirectoryInfo (pathtosourcedir);
            if (!sourcedir.Exists)
            {
                throw new DirectoryNotFoundException ();
            }
            string pkgdirpath = Path.Combine (this.PackageDir.FullName, pkg.Name);
            DirectoryInfo destdir = new DirectoryInfo (pkgdirpath);
            if (!destdir.Exists)
            {
                destdir.Create();
            }
            if (!destdir.IsEmpty ())
            {
                throw new PackageIoException ("New package directory is not empty: " + destdir.Name);
            }
            sourcedir.CopyTo (destdir.FullName, true);

            // create package definition file
            string pkgdefpath = Path.Combine(destdir.FullName, "package.wpdef");
            using (Stream str = System.IO.File.OpenWrite(pkgdefpath))
            {
                WaldiSerializer.Serialize(pkg, str);
            }
        }

		protected IPackage ReadPackageDir(DirectoryInfo dir)
		{
			if (!dir.Exists)
			{
                throw new PackageIoException ("Package directory for package " + dir.Name + " does not exist.");
			}
			FileInfo definitionfile = new FileInfo(Path.Combine(dir.FullName,"package.wpdef"));
			if (definitionfile.Exists)
			{
				using (StreamReader stream = new StreamReader(definitionfile.FullName))
				{
                    try
                    {
					    return WaldiSerializer.DeserializePackage (stream);
                    }
                    catch(Exception ex)
                    {
                        throw new PackageIoException ("Could not read package definition for package " + dir.Name + ".", dir.Name, ex);
                    }
				}
			}
			return null;
		}

		protected PackageList ReadRepositoryDir(DirectoryInfo dir)
		{
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException("Repository directory does not exist.");
			}
			PackageList list = new PackageList ();
			foreach (DirectoryInfo pdir in dir.EnumerateDirectories())
			{
				IPackage pkg = this.ReadPackageDir (pdir);
				if (pkg != null)
				{
					list.Add (pkg);
				}
			}
			return list;
		}

        public bool Equals (DirectoryPackageRepository other)
        {
            return this.Equals ((IPackageRepository)other);
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
            if (!(other is DirectoryPackageRepository))
            {
                return false;
            }
            DirectoryPackageRepository othertyped = other as DirectoryPackageRepository;
            bool areequal = string.Equals (this.Name, othertyped.Name);
            areequal = areequal && DirectoryInfoExtensions.Equals(this.PackageDir, othertyped.PackageDir);
            return areequal;
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
        }}
}