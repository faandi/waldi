using System;
using Waldi.Engine;
using Waldi.Serialization;
using Waldi.Repositories;
using System.Xml.Serialization;
using IO = System.IO;

namespace Waldi.CLI
{
    public static class Config
    {
        public static DirectoryPackageRepository LocalRepository { get; set;}
        public static DirectoryPackageRepository RemoteRepository { get; set;}

        public static bool TryLoad()
        {
            string configpath = IO.Path.Combine(Environment.CurrentDirectory, "waldiconfig.xml");
            if (!IO.File.Exists(configpath))
            {
                return false;
            }
            WaldiSerializer.MapType(typeof(ConfigDtoDto),typeof(ConfigDto));
            string serstr = System.IO.File.ReadAllText(configpath);
            ConfigDto serobj  = WaldiSerializer.Deserialize(serstr, typeof(ConfigDto), typeof(ConfigDtoDto)) as ConfigDto;
            Config.LocalRepository = serobj.LocalRepository as DirectoryPackageRepository;
            Config.RemoteRepository = serobj.RemoteRepository as DirectoryPackageRepository;
            return true;
        }
    }

    // TODO: Support für andere Repositories (über IPackageRepository)
    // dafür muss aber das Serialisieren in der Waldi.Lib angepasst werden
    public class ConfigDto
    {
        public DirectoryPackageRepository LocalRepository { get; set;}
        public DirectoryPackageRepository RemoteRepository { get; set;}
    }

    [XmlRoot("Config")]
    public class ConfigDtoDto
    {
        public DirectoryPackageRepositoryDto LocalRepository { get; set;}
        public DirectoryPackageRepositoryDto RemoteRepository { get; set;}
    }

}