using System;
using Waldi.Engine;
using Waldi.Packages;
using System.Collections.Generic;
using System.IO;
using Waldi.BclExtensions;
using Waldi.Lib;

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
                PrintInfo("Package " + pkgname + " is already in local repository. No action done.");
                return;
            }

            List<string> allpkgnames;
            try
            {
                allpkgnames = this.GetDependencies(pkgname, withdeps);
            }
            catch(PackageNotFoundException ex)
            {
                PrintError(ex);
                return;
            }
            foreach (string pname in allpkgnames)
            {
                IPackage remotepkg = this.RemoteRep.GetPackage(pname);
                if (remotepkg == null)
                {
                    PrintError("Package " + pname + " not found in remote repository.");
                    return;
                }
                string tmppkgpath;
                if (!PathExtensions.GetTempPath(out tmppkgpath))
                {
                    PrintError("Could not get a temporary directory.");
                    return;
                }
                try
                {
                    this.RemoteRep.CopyPackageFiles(pname, tmppkgpath);
                }
                catch(PackageIoException ex)
                {
                    PrintError(ex);
                    return;
                }
                if (this.LocalRep.GetPackage(pname) == null)
                {
                    this.LocalRep.AddPackage(remotepkg, tmppkgpath);
                    PrintInfo("Added package " + pname + " to local repository.");
                }
                else
                {
                    PrintInfo("Package " + pkgname + " is already in local repository.");
                }
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

        private List<string> GetDependencies(string pkgname, bool withdeps = true)
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
            if (withdeps)
            {
                foreach (Dependency d in pkg.Dependencies)
                {
                    if (!allpkgnames.Contains(d.PackageName))
                    {
                        allpkgnames.Add(d.PackageName);
                    }
                    List<string> subpkgnames = this.GetDependencies(d.PackageName, withdeps);
                    foreach (string subd in subpkgnames)
                    {
                        if (!allpkgnames.Contains(subd))
                        {
                            allpkgnames.Add(subd);
                        }
                    }
                }
            }
            // first deps, then package
            allpkgnames.Reverse();
            return allpkgnames;
        }

        public static void PrintError(Exception ex)
        {
            Console.Write("{0}Error: {1}", Environment.NewLine, ex.Message);
            if (ex.InnerException != null)
            {
                Console.Write("{0}Details: {1}", Environment.NewLine, ex.InnerException.Message);
            }
        }

        public static void PrintError(string message)
        {
            Console.Write("{0}Error: {1}", Environment.NewLine, message);
        }

        public static void PrintInfo(string message)
        {
            Console.Write("{0}{1}", Environment.NewLine, message);
        }
    }   
}

