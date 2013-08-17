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
				IO.Path.Combine("Testdata", "packagerepository", "package4", "package.wpdef")
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

			// TODO: asserts
		}

		// TODO: test for void Refresh();
	}
}
