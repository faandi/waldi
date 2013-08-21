using System;
using CommandLine;
using System.Collections.Generic;

namespace Waldi.CLI
{
	internal class PullSubOptions : CommonSubOptions
	{
		[Option('d', "dependencies", HelpText="Also pull all Dependencies.")]
        public bool WithDependencies { get; set; }
		
        [ValueList(typeof(List<string>), MaximumElements = 1)]
        public IList<string> PackageNames { get; set; }

        public bool ValidatePackageName()
        {
            return this.PackageNames != null && this.PackageNames.Count > 0;
        }
	}
}

