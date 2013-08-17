using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldi.Engine;
using Waldi.Packages;

namespace Waldi.Tests
{
    [TestFixture]
	[Category("Various")]
	public class InvalidArgumentsTests
    {
        [Test]
		public void SetInvalidFeatureName()
        {
			ArgumentException ex = Assert.Throws<ArgumentException>(
				delegate {
					Feature feature = new Feature("Invalid-Package+Name");
				}
			);
			Assert.DoesNotThrow(
				delegate {
					Feature feature = new Feature("Valid.PackageName01");
				}
			);
        }
    }
}