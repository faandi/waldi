using System;

namespace Waldi.CLI
{
    public class PackageNotFoundException : Exception
    {
        public PackageNotFoundException(string pkgname) : base("Package " + pkgname + " not found")
        {
        }

        public PackageNotFoundException(string pkgname, string msg) : base(msg)
        {
        }
    }
}