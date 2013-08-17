using System;
using System.IO;
using Waldi.Engine;
using Waldi.Packages;
using Waldi.Serialization;
using Waldi.BclExtensions;

namespace Waldi.Repositories
{
    public class DirectoryPackageRepository : IPackageRepository, IEquatable<DirectoryPackageRepository>
	{
		public string Name { get; private set;}
		public DirectoryInfo PackageDir { get; private set;}
		private PackageList packages;

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
			if (this.packages == null)
			{
				this.packages = this.ReadRepositoryDir(this.PackageDir);
			}
			return this.packages;
		}

		public IPackage GetPackage(string pkgname)
		{
			// TODO: only read needed package, not all (if not already done)
			if (this.packages == null)
			{
				this.packages = this.ReadRepositoryDir(this.PackageDir);
			}
			return this.packages.FindByName (pkgname);
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
				throw new DirectoryNotFoundException ();
			}
			DirectoryInfo destdir = new DirectoryInfo (pathtodestdir);
			if (!destdir.IsEmpty ())
			{
				throw new ArgumentException ("Directory is not empty.", "pathtodestdir");
			}
			pkgdir.CopyTo (pathtodestdir, true);
        }

		protected IPackage ReadPackageDir(DirectoryInfo dir)
		{
			if (!dir.Exists)
			{
				throw new ArgumentException ("Directory does not exist", "path");
			}
			FileInfo definitionfile = new FileInfo(Path.Combine(dir.FullName,"package.wpdef"));
			if (definitionfile.Exists)
			{
				using (StreamReader stream = new StreamReader(definitionfile.FullName))
				{
					return WaldiSerializer.DeserializePackage (stream);
				}
			}
			return null;
		}

		protected PackageList ReadRepositoryDir(DirectoryInfo dir)
		{
			if (!dir.Exists)
			{
				throw new ArgumentException ("Directory does not exist", "path");
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