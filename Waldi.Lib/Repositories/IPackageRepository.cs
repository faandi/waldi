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
		IPackage GetPackage(string pkgname);
		void Refresh();
		void CopyPackageFiles(string pkgname, string pathtodestdir);
        void AddPackage(IPackage pkg, string pathtosourcedir);
	}
}