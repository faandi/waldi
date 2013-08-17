using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Waldi.Engine;

namespace Waldi.Packages
{
	public class PackageList : NamedItemList<IPackage>
	{
        //public IEnumerable<IPackage> GetRecursive()
        //{
        //    List<IPackage> pkgs = new List<IPackage>();
        //    foreach (IPackage p in this)
        //    {
        //        pkgs.Add(p);
        //        pkgs.AddRange(p.Packages.GetRecursive());
        //    }
        //    return pkgs;
        //}
	}
}