using System;
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Waldi.CLI
{
	internal class Options
	{
        [VerbOption("pull", HelpText = "Pull packages from remote repository and saves them to the local.")]
        public PullSubOptions PullVerb { get; set; }

        [VerbOption("list", HelpText = "Lists packages from repository.")]
        public ListSubOptions ListVerb { get; set; }

        // TODO: implement build
        //[VerbOption("build", HelpText = "Build package.")]
        //public BuildSubOptions BuildVerb { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

		public Options()
		{
			// Since we create this instance the parser will not overwrite it
            PullVerb = new PullSubOptions
            {
                WithDependencies = false
            };
		}        

		// Remainder omitted
		[HelpVerbOption]
		public string GetUsage(string verb)
		{
			//if (verb == "pull")
			//{
			//}

			return HelpText.AutoBuild(this, verb);
		}  

        public string GetErrors()
        {
            HelpText help = new HelpText();

            help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));

            if (this.LastParserState.Errors.Count > 0) {
                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                if (!string.IsNullOrEmpty(errors)) {
                    help.AddPreOptionsLine(errors);
                }
            }
            return help.ToString();
        }      
	}
}

