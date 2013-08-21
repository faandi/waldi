using System;
using Waldi.Engine;
using Waldi.Serialization;
using Waldi.Repositories;
using System.Xml.Serialization;
using IO = System.IO;

namespace Waldi.CLI
{
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message)
        {
        }

        public ConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public static class Config
    {
        public static IPackageRepository LocalRepository { get; set;}
        public static IPackageRepository RemoteRepository { get; set;}

        public static void Load()
        {
            string configpath = IO.Path.Combine(Environment.CurrentDirectory, "waldiconfig.xml");
            if (!IO.File.Exists(configpath))
            {
                throw new ConfigException("Could not find configuration file in current directory.");
            }
            WaldiSerializer.MapType(typeof(ConfigDtoDto),typeof(ConfigDto));
            ConfigDto serobj;
            try
            {
                string serstr = System.IO.File.ReadAllText(configpath);
                serobj  = WaldiSerializer.Deserialize(serstr, typeof(ConfigDto), typeof(ConfigDtoDto)) as ConfigDto;
            }
            catch(Exception ex)
            {
                throw new ConfigException("Could load config from configuration file in current directory.", ex);
            }
            Config.LocalRepository = serobj.LocalRepository;
            Config.RemoteRepository = serobj.RemoteRepository;
        }
    }

    // TODO: Support für andere Repositories (über IPackageRepository)
    // dafür muss aber das Serialisieren in der Waldi.Lib angepasst werden
    public class ConfigDto
    {
        public DirectoryPackageRepository LocalRepository { get; set;}
        public MultiPackageRepository RemoteRepository { get; set;}
    }

    [XmlRoot("Config")]
    public class ConfigDtoDto
    {
        public DirectoryPackageRepositoryDto LocalRepository { get; set;}
        public MultiPackageRepositoryDto RemoteRepository { get; set;}
    }

}