using System;
using NUnit.Framework;
using Waldi.Engine;
using Waldi.Packages;
using IO = System.IO;
using System.Linq;
using Waldi.Repositories;
using Waldi.BclExtensions;

namespace Waldi.Tests
{
	[TestFixture]
	[Category("Repositories")]
	public class DirectoryPackageRepositoryTests
	{
		/// <summary>
		/// This is run once before any tests in this fixture.
		/// </summary>
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string[] filepaths = new string[] {
				IO.Path.Combine("Testdata", "packagerepository", "package1", "package.wpdef"),
				IO.Path.Combine("Testdata", "packagerepository", "package2", "package.wpdef"),
				IO.Path.Combine("Testdata", "packagerepository", "package3", "package.wpdef"),
				IO.Path.Combine("Testdata", "packagerepository", "package4", "package.wpdef"),
                IO.Path.Combine("Testdata", "newpackagefiles", "mytemplate.cshtml")
			};
			ItemDeployment.DeployFiles (filepaths, true);
		}

		[Test]
		public void GetPackages()
		{
			IPackageRepository rep = new DirectoryPackageRepository("myrep", IO.Path.Combine ("Testdata", "packagerepository"));
			PackageList packages = rep.GetPackages ();

			Assert.IsNotNull(packages);
			Assert.AreEqual(4, packages.Count ());
			Assert.IsTrue (packages.Contains ("package1"));
			Assert.IsTrue (packages.Contains ("package2"));
			Assert.IsTrue (packages.Contains ("package3"));
			Assert.IsTrue (packages.Contains ("package4"));
			Assert.IsTrue (packages["package1"].Versions.Contains("1.1"));
			Assert.IsTrue (packages["package2"].Versions.Contains("2.1"));
			Assert.IsTrue (packages ["package3"].Dependencies.Contains ("package4.feature2"));
			Assert.AreEqual ("4.4", packages ["package3"].Dependencies["package4.feature2"].Version.Name);
		}

		[Test]
		public void GetPackage()
		{
			IPackageRepository rep = new DirectoryPackageRepository("myrep", IO.Path.Combine ("Testdata", "packagerepository"));
			IPackage package = rep.GetPackage ("package1");

			Assert.IsNotNull(package);
			Assert.AreEqual("package1", package.Name);
			Assert.IsTrue (package.Versions.Contains("1.2"));
		}

		[Test]
		public void CopyPackageFiles()
		{
			IPackageRepository rep = new DirectoryPackageRepository("myrep", IO.Path.Combine ("Testdata", "packagerepository"));
			string tmpdir = null;
			if (!PathExtensions.GetTempPath (out tmpdir))
			{
				throw new Exception ("Could not get tmpdir.");
			}
			string destdir = IO.Path.Combine(tmpdir, "new_packagerepository", "package3");
			IO.Directory.CreateDirectory (destdir);
			rep.CopyPackageFiles ("package3", destdir);

            Assert.IsTrue(IO.Directory.Exists(destdir), "Package dir does not exist.");
            Assert.IsTrue(IO.File.Exists(IO.Path.Combine(destdir, "package.wpdef")), "Not all package files copied.");
		}

        [Test]
        public void AddPackage()
        {
            BasicPackage package = new BasicPackage ("mypackageNew1") {
                Description = "mydescription",
                Url = "http://myurl.test/",
                IsMaster = true,
                SelectedVersion = new PackageVersion("2.4")
            };
            package.Versions.Add (new PackageVersion ("1.0"));
            package.Versions.Add (new PackageVersion ("2.3"));
            package.Versions.Add (new PackageVersion ("2.4"));
            package.Dependencies.Add (new Dependency ("mypackage2.myfeature1"));
            package.Dependencies.Add (new Dependency ("mypackage2.myfeature2"));
            package.Dependencies.Add (new Dependency ("mypackage3.myfeature1"));
            package.Features.Add(new Feature("myfeature1"));
            package.Features.Add(new Feature("myfeature2"));

            string pkgdir = IO.Path.Combine("Testdata", "newpackagefiles");
            string repdir = IO.Path.Combine("Testdata", "packagerepository");
            IPackageRepository rep = new DirectoryPackageRepository("myrep", repdir);

            rep.AddPackage(package, pkgdir);
            rep.Refresh();

            Assert.IsTrue(IO.Directory.Exists(IO.Path.Combine(repdir, "mypackageNew1")), "Package dir does not exist.");
            Assert.IsTrue(IO.File.Exists(IO.Path.Combine(repdir, "mypackageNew1", "mytemplate.cshtml")), "Package files do not exist.");
            Assert.IsTrue(IO.File.Exists(IO.Path.Combine(repdir, "mypackageNew1", "package.wpdef")), "Package definition file does not exist.");
            Assert.IsNotNull(rep.GetPackage("mypackageNew1"), "New package not found in repository.");
        }

        [Test]
        public void Refresh()
        {
            BasicPackage package = new BasicPackage("mypackageNew1");
            string pkgdir = IO.Path.Combine("Testdata", "newpackagefiles");
            string repdir = IO.Path.Combine("Testdata", "packagerepository");
            IPackageRepository rep = new DirectoryPackageRepository("myrep", repdir);
            Assert.IsNull(rep.GetPackage("mypackageNew1"));
            rep.AddPackage(package, pkgdir);
            rep.Refresh();
            Assert.IsNotNull(rep.GetPackage("mypackageNew1"));
        }

        [TearDown]
        public void Cleanup()
        {
            string repdir = IO.Path.Combine("Testdata", "packagerepository");
            string[] files = new string[]
            {
                IO.Path.Combine(repdir, "mypackageNew1", "mytemplate.cshtml"),
                IO.Path.Combine(repdir, "mypackageNew1", "package.wpdef")
            };
            string[] dirs = new string[]
            {
                IO.Path.Combine(repdir, "mypackageNew1")
            };
            foreach (string path in files)
            {
                if (IO.File.Exists(path))
                {
                    IO.File.Delete(path);
                }
            }
            foreach (string path in dirs)
            {
                if (IO.Directory.Exists(path))
                {
                    IO.Directory.Delete(path);
                }
            }
        }
	}
}
