using System;
using System.IO;
using System.Collections.Generic;
using Waldi.Engine;
using Waldi.Packages;

namespace Waldi.Repositories
{
	public class MultiPackageRepository : IPackageRepository
	{
		private List<IPackageRepository> repositories;

		public MultiPackageRepository (IEnumerable<IPackageRepository> repositories)
		{
			this.repositories = new List<IPackageRepository> (repositories);
		}

		public MultiPackageRepository ()
		{
			this.repositories = new List<IPackageRepository>();
		}

		public void Refresh()
		{
			foreach (IPackageRepository rep in this.repositories)
			{
				rep.Refresh ();
			}
		}

		public PackageList GetPackages()
		{
			PackageList plist = new PackageList ();
			foreach (IPackageRepository rep in this.repositories)
			{
				plist.AddRange (rep.GetPackages ());
			}
			return plist;
		}

		public IPackage GetPackage(string name)
		{
			foreach (IPackageRepository rep in this.repositories)
			{
				IPackage p = rep.GetPackage (name);
				if (p == null)
				{
					return p;
				}
			}
			return null;
		}

		public void CopyPackageFiles(string pkgname, string pathtodestdir)
        {
            throw new NotImplementedException();
        }
	}
}