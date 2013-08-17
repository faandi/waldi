using System;
using System.IO;
using Waldi.Packages;
using System.Collections.Generic;

namespace Waldi.Engine
{
    public interface IPackageRepository : IEquatable<IPackageRepository>
	{	
		string Name { get; }
		PackageList GetPackages();

		//PackageList GetPackages(string name);
        //PackageList GetPackages(IEnumerable<string> names);

		IPackage GetPackage(string pkgname);
		void Refresh();
		void CopyPackageFiles(string pkgname, string pathtodestdir);
	}
}