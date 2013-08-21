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
	public class MultiPackageRepositoryTests
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
				IO.Path.Combine("Testdata", "packagerepository2", "package21", "package.wpdef"),
                IO.Path.Combine("Testdata", "packagerepository2", "package22", "package.wpdef")
			};
			ItemDeployment.DeployFiles (filepaths, true);
		}

		[Test]
		public void GetPackages()
		{
			IPackageRepository rep1 = new DirectoryPackageRepository("myrep1", IO.Path.Combine ("Testdata", "packagerepository"));
			IPackageRepository rep2 = new DirectoryPackageRepository("myrep2", IO.Path.Combine ("Testdata", "packagerepository2"));
			IPackageRepository rep = new MultiPackageRepository ("myrep", rep1, rep2);

			PackageList packages = rep.GetPackages ();

			Assert.IsNotNull(packages);
			Assert.AreEqual(6, packages.Count ());
			Assert.IsTrue (packages.Contains ("package1"));
			Assert.IsTrue (packages.Contains ("package2"));
			Assert.IsTrue (packages.Contains ("package3"));
			Assert.IsTrue (packages.Contains ("package4"));
			Assert.IsTrue (packages.Contains ("package21"));
			Assert.IsTrue (packages.Contains ("package22"));
			Assert.IsTrue (packages["package1"].Versions.Contains("1.1"));
			Assert.IsTrue (packages["package2"].Versions.Contains("2.1"));
			Assert.IsTrue (packages ["package3"].Dependencies.Contains ("package4.feature2"));
			Assert.AreEqual ("4.4", packages ["package3"].Dependencies["package4.feature2"].Version.Name);
		}

		[Test]
		public void GetPackage()
		{
			IPackageRepository rep1 = new DirectoryPackageRepository("myrep1", IO.Path.Combine ("Testdata", "packagerepository"));
			IPackageRepository rep2 = new DirectoryPackageRepository("myrep2", IO.Path.Combine ("Testdata", "packagerepository2"));
			IPackageRepository rep = new MultiPackageRepository ("myrep", rep1, rep2);
			// rep1
			IPackage package = rep.GetPackage ("package1");
			Assert.IsNotNull(package);
			Assert.AreEqual("package1", package.Name);
			Assert.IsTrue (package.Versions.Contains("1.2"));
			// rep2
			package = rep.GetPackage ("package22");
			Assert.IsNotNull(package);
			Assert.AreEqual("package22", package.Name);
			Assert.IsTrue (package.Versions.Contains("2.2"));
		}

		[Test]
		public void CopyPackageFiles()
		{
			IPackageRepository rep1 = new DirectoryPackageRepository("myrep1", IO.Path.Combine ("Testdata", "packagerepository"));
			IPackageRepository rep2 = new DirectoryPackageRepository("myrep2", IO.Path.Combine ("Testdata", "packagerepository2"));
			IPackageRepository rep = new MultiPackageRepository ("myrep", rep1, rep2);

			string tmpdir = null;
			if (!PathExtensions.GetTempPath (out tmpdir))
			{
				throw new Exception ("Could not get tmpdir.");
			}
			// rep1
			string destdir = IO.Path.Combine(tmpdir, "new_packagerepository", "package3");
			IO.Directory.CreateDirectory (destdir);
			rep.CopyPackageFiles ("package3", destdir);
            Assert.IsTrue(IO.Directory.Exists(destdir), "Package dir does not exist.");
            Assert.IsTrue(IO.File.Exists(IO.Path.Combine(destdir, "package.wpdef")), "Not all package files copied.");
			// rep2
			destdir = IO.Path.Combine(tmpdir, "new_packagerepository", "package22");
			IO.Directory.CreateDirectory (destdir);
			rep.CopyPackageFiles ("package22", destdir);
			Assert.IsTrue(IO.Directory.Exists(destdir), "Package dir does not exist.");
			Assert.IsTrue(IO.File.Exists(IO.Path.Combine(destdir, "package.wpdef")), "Not all package files copied.");
		}

        [Test]
        public void AddPackage()
        {
			IPackageRepository rep1 = new DirectoryPackageRepository("myrep1", IO.Path.Combine ("Testdata", "packagerepository"));
			IPackageRepository rep2 = new DirectoryPackageRepository("myrep2", IO.Path.Combine ("Testdata", "packagerepository2"));
			IPackageRepository rep = new MultiPackageRepository ("myrep", rep1, rep2);

			BasicPackage package = new BasicPackage ("mypackageNew1");            

            string tmpdir = null;
            if (!PathExtensions.GetTempPath (out tmpdir))
            {
                throw new Exception ("Could not get tmpdir.");
            }

			Assert.Throws<NotImplementedException>(
				delegate {
                    rep.AddPackage(package, tmpdir);
				}
			);
        }
	}
}