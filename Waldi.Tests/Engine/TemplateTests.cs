using System;
using IO = System.IO;
using NUnit.Framework;
using Waldi.Engine;
using Waldi.Packages;
using Waldi.BclExtensions;

namespace Waldi.Tests
{
    public class TemplateTests
    {
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            string[] filepaths = new string[] {
                IO.Path.Combine("Testdata", "razortemplates", "simplestrongly", "hellopackageStrongly.cshtml"),
                IO.Path.Combine("Testdata", "razortemplates", "simpleweakly", "hellopackageWeakly.cshtml"),
                IO.Path.Combine("Testdata", "razortemplates", "simplewithlayout", "hellopackage.cshtml"),
                IO.Path.Combine("Testdata", "razortemplates", "simplewithlayout", "layout.cshtml")
            };
            ItemDeployment.DeployFiles (filepaths, true);
        }

        [Test]
        public void CompileSimpleStrongly()
        {
            BasicPackage pkg = new BasicPackage("mycoolpackage");
            string tpldir = IO.Path.Combine("Testdata", "razortemplates", "simplestrongly");
            string outpath;
            if (!PathExtensions.GetTempPath(out outpath))
            {
                throw new Exception("Could not get temp path.");
            }
            outpath = IO.Path.Combine(outpath, "result.html");
            Template.CompileFile(tpldir, "hellopackageStrongly", pkg, outpath);
            string viewcontent = IO.File.ReadAllText(outpath);
            string expectedcontent = string.Format("<html>{0}\t<head>{0}\t\t<title>Simple Template (strongly typed model)</title>{0}\t</head>{0}\t<body>{0}\t\tHello mycoolpackage!{0}\t</body>{0}</html>", Environment.NewLine);
            Assert.AreEqual(expectedcontent, viewcontent);
        }

        [Test]
        public void CompileSimpleWeakly()
        {
            BasicPackage pkg = new BasicPackage("mycoolpackage");
            string tpldir = IO.Path.Combine("Testdata", "razortemplates", "simpleweakly");
            string outpath;
            if (!PathExtensions.GetTempPath(out outpath))
            {
                throw new Exception("Could not get temp path.");
            }
            outpath = IO.Path.Combine(outpath, "result.html");
            Template.CompileFile(tpldir, "hellopackageWeakly", pkg, outpath);
            string viewcontent = IO.File.ReadAllText(outpath);
            string expectedcontent = string.Format("<html>{0}\t<head>{0}\t\t<title>Simple Template (weakly typed model)</title>{0}\t</head>{0}\t<body>{0}\t\tHello mycoolpackage!{0}\t</body>{0}</html>", Environment.NewLine);
            Assert.AreEqual(expectedcontent, viewcontent);
        }

        [Test]
        public void CompileSimpleWithLayout()
        {
            BasicPackage pkg = new BasicPackage("mycoolpackage");
            string tpldir = IO.Path.Combine("Testdata", "razortemplates", "simplewithlayout");
            string outpath;
            if (!PathExtensions.GetTempPath(out outpath))
            {
                throw new Exception("Could not get temp path.");
            }
            outpath = IO.Path.Combine(outpath, "result.html");
            Template.CompileFile(tpldir, "hellopackage", pkg, outpath);
            string viewcontent = IO.File.ReadAllText(outpath);
            string expectedcontent = string.Format("<html>{0}\t<head>{0}\t\t<title>Simple Template with layout</title>{0}\t</head>{0}\t<body>{0}\t\t{0}Hello mycoolpackage!{0}\t</body>{0}</html>", Environment.NewLine);
            Assert.AreEqual(expectedcontent, viewcontent);
        }
    }
}

