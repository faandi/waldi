using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Waldi.Packages;
using Waldi.Engine;

namespace Waldi.Tests
{
    [TestFixture]
	[Category("Package")]
	public class DependencyResolverTests
    {
		/// <summary>
		/// This is run once before any tests in this fixture.
		/// </summary>
		[TestFixtureSetUp]
		public void SetUpFixture()
		{

		}

		private PackageList GetPackages()
		{
			BasicPackage p1 = new BasicPackage ("package1") {
				Description = "mydescription_package1",
				IsMaster = false,
				Url = "http://myurl_package1.com"
			};
            p1.Versions.Add (new PackageVersion("1.1"));
            p1.Versions.Add (new PackageVersion("1.2"));

			BasicPackage p2 = new BasicPackage ("package2") {
				Description = "mydescription_package2",
				IsMaster = false,
				Url = "http://myurl_package2.com"
			};
            p2.Versions.Add (new PackageVersion("2.1"));
            p2.Versions.Add (new PackageVersion("2.2"));
			p2.Dependencies.Add (new Dependency ("package1") {
                Version = new PackageVersion("1.1")
			});
			p2.Features.Add (new Feature ("feature1") {
				Description = "description_feature1",
				Url = "http://myurl_feature1.com/feat",
				Enabled = false
			});

			BasicPackage p3 = new BasicPackage ("package3") {
				Description = "mydescription_package3",
				IsMaster = false,
				Url = "http://myurl_package3.com"
			};
            p3.Versions.Add (new PackageVersion("3.1"));
            p3.Versions.Add(new PackageVersion("3.2"));
            p3.Versions.Add(new PackageVersion("3.3"));
			p3.Dependencies.Add (new Dependency ("package2.feature1") {
                Version = new PackageVersion("2.2")
			});
			p3.Dependencies.Add (new Dependency ("package1") {
                Version = new PackageVersion("1.1")
			});
			p3.Dependencies.Add (new Dependency ("package4.feature2") {
                    Version = new PackageVersion("4.4")
			});
			p3.Features.Add (new Feature ("feature1") {
				Description = "description_feature1",
				Url = "http://myurl_feature1.com/feat",
				Enabled = false
			});
			p3.Features.Add (new Feature ("feature2") {
				Description = "description_feature2",
				Url = "http://myurl_feature2.com/feat",
				Enabled = false
			});

			BasicPackage p4 = new BasicPackage ("package4") {
				Description = "mydescription_package4",
				IsMaster = false,
				Url = "http://myurl_package4.com"
			};
            p4.Versions.Add (new PackageVersion("4.1"));
            p4.Versions.Add(new PackageVersion("4.4"));
			p4.Dependencies.Add (new Dependency ("package2.feature1") {
                Version = new PackageVersion("2.2")
			});
			p4.Dependencies.Add (new Dependency ("package1") {
                    Version = new PackageVersion("1.1")
			});
			p4.Features.Add (new Feature ("feature1") {
				Description = "description_feature1",
				Url = "http://myurl_feature1.com/feat",
				Enabled = false
			});
			p4.Features.Add (new Feature ("feature2") {
				Description = "description_feature2",
				Url = "http://myurl_feature2.com/feat",
				Enabled = false
			});

			PackageList packages = new PackageList ();
			packages.Add (p1);
			packages.Add (p2);
			packages.Add (p3);
			packages.Add (p4);
			return packages;
		}

        [Test]
		public void OrderByDependencies()
        {
			PackageList packages = this.GetPackages ();
			List<IPackage> orderedpacks = DependencyResolver.OrderByDependencies (packages);

			Assert.AreEqual (4, orderedpacks.Count());
			Assert.AreEqual ("package1", orderedpacks[0].Name);
			Assert.AreEqual ("package2", orderedpacks[1].Name);
			Assert.AreEqual ("package4", orderedpacks[2].Name);
			Assert.AreEqual ("package3", orderedpacks[3].Name);
        }

		[Test]
		public void AddDependencePackages()
		{
			PackageList packages = this.GetPackages ();
			DependencyResolver.AddDependencePackages (packages);

			Assert.AreEqual (1, packages ["package2"].Packages.Count());
			Assert.IsTrue (packages ["package2"].Packages.Contains ("package1"));
			Assert.AreEqual (3, packages ["package3"].Packages.Count());
			Assert.IsTrue (packages ["package3"].Packages.Contains ("package2"));
			Assert.IsTrue (packages ["package3"].Packages.Contains ("package1"));
			Assert.IsTrue (packages ["package3"].Packages.Contains ("package4"));
			Assert.AreEqual (2, packages ["package4"].Packages.Count());
			Assert.IsTrue (packages ["package4"].Packages.Contains ("package2"));
			Assert.IsTrue (packages ["package4"].Packages.Contains ("package1"));
		}

        [Test]
        public void EnableFeatures()
        {
            PackageList packages = this.GetPackages ();
            DependencyResolver.AddDependencePackages (packages);
            DependencyResolver.EnableFeatures (packages);

            Assert.IsTrue(packages ["package2"].Features["feature1"].Enabled);
            Assert.IsTrue(packages ["package4"].Features["feature2"].Enabled);
            Assert.IsFalse(packages ["package3"].Features["feature1"].Enabled);
            Assert.IsFalse(packages ["package3"].Features["feature2"].Enabled);
            Assert.IsFalse(packages ["package4"].Features["feature1"].Enabled);
        }

        [Test]
        public void SelectVersions()
        {
            PackageList packages = this.GetPackages ();
            DependencyResolver.SelectVersions(packages);

            Assert.NotNull(packages["package1"].SelectedVersion);
            Assert.NotNull(packages["package2"].SelectedVersion);
            Assert.NotNull(packages["package4"].SelectedVersion);
            Assert.AreEqual ("1.1", packages ["package1"].SelectedVersion.Number);
            Assert.AreEqual ("2.2", packages ["package2"].SelectedVersion.Number);
            Assert.AreEqual ("4.4", packages ["package4"].SelectedVersion.Number);
        }
    }
}
