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
		public static string CompileTemplate(string templatestring, IPackage package)
		{
			if (package == null) {
				throw new ArgumentNullException ("package");
			}
			if (string.IsNullOrEmpty (templatestring)) {
				throw new ArgumentException ("templatestring cannot be null or empty.", "templatestring");
			}
			string result = Razor.Parse(templatestring, package);
			return result;
		}

		public static void CompileFile(string templatepath, string outpath, IPackage package)
		{
			if (package == null) {
				throw new ArgumentNullException ("package");
			}
			if (string.IsNullOrEmpty (templatepath)) {
				throw new ArgumentException ("templatepath cannot be null or empty.", "templatepath");
			}
			if (string.IsNullOrEmpty (outpath)) {
				throw new ArgumentException ("outpath cannot be null or empty.", "outpath");
			}
			string template = IO.File.ReadAllText (templatepath);
			string result = Razor.Parse(template, package);
			IO.File.WriteAllText (outpath, result);
		}

        public static void CompileFile(string templatepath, IEnumerable<string> parentpaths, string outpath, IPackage package)
        {
            using (TemplateService service = new TemplateService())
            {
                foreach (string path in parentpaths)
                {
                    string tpl = IO.File.ReadAllText (path);
                    service.Compile(tpl, typeof(object), IO.Path.GetFileNameWithoutExtension(path));
                }
                string tplstring = IO.File.ReadAllText (templatepath);
                string result = service.Parse(tplstring, package, null, null);
                IO.File.WriteAllText (outpath, result);
            }
        }

        public static void Compile()
        {

        }
    }
}
