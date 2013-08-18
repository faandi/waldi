using System;
using Waldi.Engine;
using Waldi.Packages;
using System.Collections.Generic;
using System.IO;
using Waldi.BclExtensions;

namespace Waldi.CLI
{
    public class Runner
    {
        public IPackageRepository LocalRep { get; set; }
        public IPackageRepository RemoteRep { get; set; }

        public void Pull(string pkgname, bool withdeps)
        {
            IPackage localpkg = this.LocalRep.GetPackage(pkgname);
            if (localpkg != null)
            {
                Console.Write("{0}Package " + pkgname + " is already in local repository. No action done", Environment.NewLine);
                return;
            }

            List<string> allpkgnames;
            try
            {
                allpkgnames = this.GetDependencies(pkgname, withdeps);
            }
            catch(PackageNotFoundException ex)
            {
                Console.Write("Error: " + ex.Message);
                return;
            }
            foreach (string pname in allpkgnames)
            {
                IPackage remotepkg = this.RemoteRep.GetPackage(pname);
                if (remotepkg == null)
                {
                    Console.Write("{0}Error: Package " + pname + " not found in remote repository.", Environment.NewLine);
                    return;
                }
                string tmppkgpath;
                if (!PathExtensions.GetTempPath(out tmppkgpath))
                {
                    throw new Exception("Could not get a temporary directory.");
                }
                this.RemoteRep.CopyPackageFiles(pname, tmppkgpath);
                this.LocalRep.AddPackage(remotepkg, tmppkgpath);
                Console.Write("{0}Added package " + pname + " to local repository.", Environment.NewLine);
            }
            Console.Write("{0}", Environment.NewLine);
        }

        public void List(bool listlocal, bool listremote, bool showdetails = false)
        {
            if (!listlocal && !listremote)
            {
                listlocal = true;
            }
            if (listlocal)
            {
                Console.Write("{0}Local packages:{0}", Environment.NewLine);
                this.PrintPackageList(this.LocalRep.GetPackages(), showdetails);
            }
            if (listremote)
            {
                Console.Write("{0}Remote packages:{0}", Environment.NewLine);
                this.PrintPackageList(this.RemoteRep.GetPackages(), showdetails);
            }
        }

        private void PrintPackageList(PackageList pkgs, bool showdetails)
        {
            bool haspkgs = false;
            foreach (IPackage p in pkgs)
            {
                this.PrintPackage(p, showdetails);
                Console.Write(Environment.NewLine);
                haspkgs = true;
            }
            if (!haspkgs)
            {
                Console.Write("No Packages.{0}", Environment.NewLine);
            }
        }

        private void PrintPackage(IPackage pkg, bool showdetails)
        {
            Console.Write("{0} - {1}", pkg.Name, pkg.Description);
        }

        private List<string> GetDependencies(string pkgname, bool recursive = true)
        {
            IPackage pkg = this.RemoteRep.GetPackage(pkgname);
            if (pkg == null)
            {
                throw new PackageNotFoundException(pkgname);
            }
            List<string> allpkgnames = new List<string>()
            {
                pkg.Name
            };
            foreach (Dependency d in pkg.Dependencies)
            {
                if (!allpkgnames.Contains(d.Name))
                {
                    allpkgnames.Add(d.PackageName);
                }
                if (recursive)
                {
                    List<string> subpkgnames = this.GetDependencies(d.Name, recursive);
                    foreach (string sd in subpkgnames)
                    {
                        if (!allpkgnames.Contains(d.Name))
                        {
                            allpkgnames.Add(d.PackageName);
                        }
                    }
                }
            }
            return allpkgnames;
        }

    }   
}

