using System;
using Waldi.Repositories;

namespace Waldi.CLI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            object invokedVerbInstance = null;

            Options options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(
                args, options,(verb, subOptions) =>
            {
                invokedVerbInstance = subOptions;
            }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            if (invokedVerbInstance is PullSubOptions)
            {
                PullSubOptions subOptions = (PullSubOptions)invokedVerbInstance;
                if (subOptions.ValidatePackageName())
                {
                    Config.Load();
                    Runner runner = new Runner()
                    {
                        LocalRep = Config.LocalRepository,
                        RemoteRep = Config.RemoteRepository
                    };
                    runner.Pull(subOptions.PackageNames[0], subOptions.WithDependencies);
                }
                else
                {
                    Console.WriteLine(options.GetUsage(null));
                }
            }
            else if (invokedVerbInstance is ListSubOptions)
            {
                ListSubOptions subOptions = (ListSubOptions)invokedVerbInstance;
                //if (subOptions.ValidateRepositoryName())
                //{
                    Config.Load();
                    Runner runner = new Runner()
                    {
                        LocalRep = Config.LocalRepository,
                        RemoteRep = Config.RemoteRepository
                    };
                    runner.List(subOptions.ListLocal, subOptions.ListRemote);
                //}
                //else
                //{
                //    Console.WriteLine(options.GetUsage(null));
                //}
            }
            else
            {
                Console.WriteLine("No Verb.");
            }

		}
	}
}
