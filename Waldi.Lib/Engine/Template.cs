using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine;
using Waldi.Packages;
using IO = System.IO;
using RazorEngine.Templating;

namespace Waldi.Engine
{
    public class Template
    {
		//public static string CompileTemplate(string templatestring, IPackage package)
		//{
		//	if (package == null) {
		//		throw new ArgumentNullException ("package");
		//	}
		//	if (string.IsNullOrEmpty (templatestring)) {
		//		throw new ArgumentException ("templatestring cannot be null or empty.", "templatestring");
		//	}
		//	string result = Razor.Parse(templatestring, package);
		//	return result;
		//}

		//public static void CompileFile(string templatepath, string outpath, IPackage package)
		//{
		//	if (package == null) {
		//		throw new ArgumentNullException ("package");
		//	}
		//	if (string.IsNullOrEmpty (templatepath)) {
		//		throw new ArgumentException ("templatepath cannot be null or empty.", "templatepath");
		//	}
		//	if (string.IsNullOrEmpty (outpath)) {
		//		throw new ArgumentException ("outpath cannot be null or empty.", "outpath");
		//	}
		//	string template = IO.File.ReadAllText (templatepath);
		//	string result = Razor.Parse(template, package);
		//	IO.File.WriteAllText (outpath, result);
		//}

        //public static void CompileFile(string templatepath, IEnumerable<string> parentpaths, string outpath, IPackage package)
        //{
        //    using (TemplateService service = new TemplateService())
        //    {
        //        foreach (string path in parentpaths)
        //        {
        //            string tpl = IO.File.ReadAllText (path);
        //            service.Compile(tpl, typeof(object), IO.Path.GetFileNameWithoutExtension(path));
        //        }
        //        string tplstring = IO.File.ReadAllText (templatepath);
        //        string result = service.Parse(tplstring, package, null, null);
        //        IO.File.WriteAllText (outpath, result);
        //    }
        //}

        public static void CompileFile(string templatedir, string viewname, IPackage package, string outpath)
        {
            IO.DirectoryInfo indir = new IO.DirectoryInfo(templatedir);
            IEnumerable<IO.FileInfo> allviewfiles = indir.GetFiles("*.cshtml", IO.SearchOption.AllDirectories);
            using (TemplateService service = new TemplateService())
            {
                IO.FileInfo viewfile = null;
                foreach (IO.FileInfo f in allviewfiles)
                {
                    string fname = IO.Path.GetFileNameWithoutExtension(f.FullName);
                    if (fname != viewname)
                    {
                        string tpl = IO.File.ReadAllText(f.FullName);
                        service.Compile(tpl, typeof(object), fname);
                    }
                    else
                    {
                        viewfile = f;
                    }
                }
                if (viewfile == null)
                {
                    throw new ArgumentException("A view named " + viewname + " could not be found.", "viewname");
                }
                string tplstring = IO.File.ReadAllText (viewfile.FullName);
                string result = service.Parse(tplstring, package, null, null);
                IO.File.WriteAllText (outpath, result);
            }
        }
    }
}
