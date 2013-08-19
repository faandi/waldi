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
        // disable "value is never used" compiler warnings
        #pragma warning disable 219

        [Test]
		public void SetInvalidFeatureName()
        {
			Assert.Throws<ArgumentException>(
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