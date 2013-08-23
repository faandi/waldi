using System;
using Waldi.Repositories;

namespace Waldi.CLI
{
	class MainClass
	{
        enum ExitCode : int {
            NoError = 0,
            GeneralError = 1,
            ConfigError = 2,
            ArgumentError = 3
        }

		public static int Main (string[] args)
		{
            object invokedVerbInstance = null;

            Options options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(
                args, options,(verb, subOptions) =>
            {
                invokedVerbInstance = subOptions;
            }))
            {
                return (int)ExitCode.ArgumentError;
            }

            if (invokedVerbInstance is PullSubOptions)
            {
                PullSubOptions subOptions = (PullSubOptions)invokedVerbInstance;
                if (subOptions.ValidatePackageName())
                {
                    try 
                    {
                        Config.Load();
                    }
                    catch(ConfigException ex)
                    {
                        Runner.PrintError(ex.Message);
                        return (int)ExitCode.ConfigError;
                    }
                    Runner runner = new Runner()
                    {
                        LocalRep = Config.LocalRepository,
                        RemoteRep = Config.RemoteRepository
                    };
                    //try 
                    //{
                    runner.Pull(subOptions.PackageNames[0], subOptions.WithDependencies);
                    Console.WriteLine();
                    //}
                    //catch(Exception ex)
                    //{
                    //    Runner.PrintError(ex.Message);
                    //    return (int)ExitCode.GeneralError;
                    //}
                    return (int)ExitCode.NoError;
                }
                else
                {
                    Console.WriteLine(options.GetUsage(null));
                    return (int)ExitCode.ArgumentError;
                }
            }
            else if (invokedVerbInstance is ListSubOptions)
            {
                ListSubOptions subOptions = (ListSubOptions)invokedVerbInstance;
                //if (subOptions.ValidateRepositoryName())
                //{
                try 
                {
                    Config.Load();
                }
                catch(ConfigException ex)
                {
                    Runner.PrintError(ex.Message);
                    return (int)ExitCode.ConfigError;
                }
                Runner runner = new Runner()
                {
                    LocalRep = Config.LocalRepository,
                    RemoteRep = Config.RemoteRepository
                };
                try
                {
                    runner.List(subOptions.ListLocal, subOptions.ListRemote);
                    return (int)ExitCode.NoError;
                }
                catch(Exception ex)
                {
                    Runner.PrintError(ex.Message);
                    return (int)ExitCode.GeneralError;
                }
                //}
                //else
                //{
                //    Console.WriteLine(options.GetUsage(null));
                //}
            }
            else
            {
                Console.WriteLine("No Verb.");
                return (int)ExitCode.ArgumentError;
            }
		}
	}
}
