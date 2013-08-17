using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldi.Engine;
using Waldi.Packages;
using Waldi.Serialization;
using Waldi.Repositories;

namespace Waldi.Tests
{
    [TestFixture]
	[Category("Serialization")]
	public class SerializationTests
    {

//		[TestFixtureSetUp]
//		public void SetUpFixture()
//		{
//			string[] filepaths = new string[] {
//				Path.Combine("Testdata", "Serialize", "package.wfdef"),
//				Path.Combine("Testdata", "Serialize", "package.wpdef"),
//				Path.Combine("Testdata", "Serialize", "package.wpcfg")
//			};
//			ItemDeployment.DeployFiles (filepaths);
//		}
    
        [Test]
		public void SerializeFeature()
        {
			Feature feature = new Feature ("myfeature1") {
				Description = "mydescription",
				Url = "http://myurl.test/",
				Enabled = true
			};
			string featurestring = WaldiSerializer.Serialize (feature);
			string expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Feature xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Name>myfeature1</Name>\r\n  <Description>mydescription</Description>\r\n  <Url>http://myurl.test/</Url>\r\n  <Enabled>true</Enabled>\r\n</Feature>";
			Assert.AreEqual (expected, featurestring);
        }

		[Test]
		public void SerializePackage()
		{
			BasicPackage package = new BasicPackage ("mypackage1") {
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

			string packagestring = WaldiSerializer.Serialize (package);
			string expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Package xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Name>mypackage1</Name>\r\n  <Description>mydescription</Description>\r\n  <Url>http://myurl.test/</Url>\r\n  <IsMaster>true</IsMaster>\r\n  <Versions>\r\n    <Version>\r\n      <Name>1.0</Name>\r\n    </Version>\r\n    <Version>\r\n      <Name>2.3</Name>\r\n    </Version>\r\n    <Version>\r\n      <Name>2.4</Name>\r\n    </Version>\r\n  </Versions>\r\n  <Dependencies>\r\n    <Dependency>\r\n      <Name>mypackage2.myfeature1</Name>\r\n      <Version>\r\n        <Name>*</Name>\r\n      </Version>\r\n      <IsOptional>false</IsOptional>\r\n    </Dependency>\r\n    <Dependency>\r\n      <Name>mypackage2.myfeature2</Name>\r\n      <Version>\r\n        <Name>*</Name>\r\n      </Version>\r\n      <IsOptional>false</IsOptional>\r\n    </Dependency>\r\n    <Dependency>\r\n      <Name>mypackage3.myfeature1</Name>\r\n      <Version>\r\n        <Name>*</Name>\r\n      </Version>\r\n      <IsOptional>false</IsOptional>\r\n    </Dependency>\r\n  </Dependencies>\r\n  <SelectedVersion>\r\n    <Name>2.4</Name>\r\n  </SelectedVersion>\r\n  <Features>\r\n    <Feature>\r\n      <Name>myfeature1</Name>\r\n      <Enabled>false</Enabled>\r\n    </Feature>\r\n    <Feature>\r\n      <Name>myfeature2</Name>\r\n      <Enabled>false</Enabled>\r\n    </Feature>\r\n  </Features>\r\n</Package>";
			Assert.AreEqual (expected, packagestring);
		}

        [Test]
        public void SerializeRepository()
        {
            // just make shure test works on different platforms
            string reppath = Path.GetTempPath();
            string reppathuri = (new Uri(reppath)).AbsoluteUri;
            // here comes the test
            DirectoryPackageRepository repository = new DirectoryPackageRepository("myrep", reppath);
            string repositorystring = WaldiSerializer.Serialize (repository);
            string expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PackageRepository xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Name>myrep</Name>\r\n  <PackageDir>" + reppathuri + "</PackageDir>\r\n</PackageRepository>";
            Assert.AreEqual (expected, repositorystring);
        }

		[Test]
		public void DeserializeFeature()
		{
			string featurestring = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Feature xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Name>myfeature1</Name>\r\n  <Description>mydescription</Description>\r\n  <Url>http://myurl.test/</Url>\r\n  <Enabled>true</Enabled>\r\n</Feature>";
			Feature feature = WaldiSerializer.DeserializeFeature (featurestring);
			Feature expected = new Feature ("myfeature1") {
				Description = "mydescription",
				Url = "http://myurl.test/",
				Enabled = true
			};
			Assert.AreEqual (expected, feature);
		}

		[Test]
		public void DeserializePackage()
		{
			string packagestring = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Package xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Name>mypackage1</Name>\r\n  <Description>mydescription</Description>\r\n  <Url>http://myurl.test/</Url>\r\n  <IsMaster>true</IsMaster>\r\n  <Versions>\r\n    <Version>\r\n      <Name>1.0</Name>\r\n    </Version>\r\n    <Version>\r\n      <Name>2.3</Name>\r\n    </Version>\r\n    <Version>\r\n      <Name>2.4</Name>\r\n    </Version>\r\n  </Versions>\r\n  <Dependencies>\r\n    <Dependency>\r\n      <Name>mypackage2.myfeature1</Name>\r\n      <Version>\r\n        <Name>*</Name>\r\n      </Version>\r\n      <IsOptional>false</IsOptional>\r\n    </Dependency>\r\n    <Dependency>\r\n      <Name>mypackage2.myfeature2</Name>\r\n      <Version>\r\n        <Name>*</Name>\r\n      </Version>\r\n      <IsOptional>false</IsOptional>\r\n    </Dependency>\r\n    <Dependency>\r\n      <Name>mypackage3.myfeature1</Name>\r\n      <Version>\r\n        <Name>*</Name>\r\n      </Version>\r\n      <IsOptional>false</IsOptional>\r\n    </Dependency>\r\n  </Dependencies>\r\n  <SelectedVersion>\r\n    <Name>2.4</Name>\r\n  </SelectedVersion>\r\n  <Features>\r\n    <Feature>\r\n      <Name>myfeature1</Name>\r\n      <Enabled>false</Enabled>\r\n    </Feature>\r\n    <Feature>\r\n      <Name>myfeature2</Name>\r\n      <Enabled>false</Enabled>\r\n    </Feature>\r\n  </Features>\r\n</Package>";
			IPackage package = WaldiSerializer.DeserializePackage (packagestring);
			BasicPackage expected = new BasicPackage ("mypackage1") {
				Description = "mydescription",
				Url = "http://myurl.test/",
				IsMaster = true,
				SelectedVersion = new PackageVersion("2.4")
			};
			expected.Versions.Add (new PackageVersion ("1.0"));
			expected.Versions.Add (new PackageVersion ("2.3"));
			expected.Versions.Add (new PackageVersion ("2.4"));
			expected.Dependencies.Add (new Dependency ("mypackage2.myfeature1"));
			expected.Dependencies.Add (new Dependency ("mypackage2.myfeature2"));
			expected.Dependencies.Add (new Dependency ("mypackage3.myfeature1"));
			expected.Features.Add(new Feature("myfeature1"));
			expected.Features.Add(new Feature("myfeature2"));
			Assert.AreEqual (expected as IPackage, package as IPackage);
		}

        [Test]
        public void DeserializeRepository()
        {
            // just make shure test works on different platforms
            string reppath = Path.GetTempPath();
            string reppathuri = (new Uri(reppath)).AbsoluteUri;
            // here comes the test
            string repositorystring = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PackageRepository xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Name>myrep</Name>\r\n  <PackageDir>" + reppathuri + "</PackageDir>\r\n</PackageRepository>";
            DirectoryPackageRepository repository = WaldiSerializer.DeserializePackageRepository (repositorystring) as DirectoryPackageRepository;
            DirectoryPackageRepository expected = new DirectoryPackageRepository("myrep", Path.GetTempPath());
            Assert.AreEqual (expected, repository);
        }

        [Test]
        public void DeserializeRepositoryLocalUri()
        {
            string repositorystring = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PackageRepository xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Name>myrep</Name>\r\n  <PackageDir>file://~/packages/</PackageDir>\r\n</PackageRepository>";
            string packagedir = Path.Combine(Directory.GetCurrentDirectory(), "packages");
            if (!Directory.Exists(packagedir))
            {
                Directory.CreateDirectory(packagedir);
            }
            DirectoryPackageRepository repository = WaldiSerializer.DeserializePackageRepository (repositorystring) as DirectoryPackageRepository;
            DirectoryPackageRepository expected = new DirectoryPackageRepository("myrep", packagedir);
            Assert.AreEqual (expected, repository);
        }
    }
}