using System;
using CommandLine;
using System.Collections.Generic;

namespace Waldi.CLI
{
	internal class ListSubOptions : CommonSubOptions
	{
        // lieber eigenes command machen ?
        // oder waldi list pkgname für details ?
		//[Option('d', "details", HelpText="Show details.")]
        //public bool ShowDetails { get; set; }

        [Option('r', "listremote", HelpText="List remote packages.")]
        public bool ListRemote { get; set; }
		
        [Option('l', "listlocal", HelpText="List local packages (default).")]
        public bool ListLocal { get; set; }

        //[ValueList(typeof(List<string>), MaximumElements = 1)]
        //public IList<string> RepositoryNames { get; set; }
        //
        //public bool ValidateRepositoryName()
        //{
        //    return this.RepositoryNames != null && this.RepositoryNames.Count > 0;
        //}
	}
}

