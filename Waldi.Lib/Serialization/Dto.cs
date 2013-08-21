using System;
using System.Xml.Serialization;
using Waldi.Packages;
using AutoMapper;
using System.Xml;
using System.IO;
using Waldi.Engine;
using System.Collections.Generic;
using System.Xml.Schema;

namespace Waldi.Serialization
{
	[XmlRoot("Feature")]
	public class FeatureDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public bool Enabled { get; set; }
	}

	[XmlRoot("Package")]
	public class PackageDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public bool IsMaster { get; set; }
		[XmlArrayItem("Version")]
		public List<PackageVersionDto> Versions { get; set; }
		[XmlArrayItem("Dependency")]
		public List<DependencyDto> Dependencies { get; set; }
		public PackageVersionDto SelectedVersion { get; set; }
		[XmlArrayItem("Feature")]
		public List<FeatureDto> Features { get; set; }
	}

	[XmlRoot("Version")]
	public class PackageVersionDto : INamedItem
	{
		public string Name { get; set; }
	}

	[XmlRoot("Dependency")]
	public class DependencyDto
	{
		public string Name { get; set; }
		public PackageVersionDto Version { get; set; }
		public bool IsOptional { get; set; }
	}

    [XmlRoot("PackageRepository")]
    public class PackageRepositoryDto
    {
    }

    [XmlRoot("DirectoryPackageRepository")]
    public class DirectoryPackageRepositoryDto : PackageRepositoryDto
    {
        public string Name { get; set; }
        public string PackageDir { get; set; }
    }

    [XmlRoot("MultiPackageRepository")]
    public class MultiPackageRepositoryDto : PackageRepositoryDto
    {
        public string Name { get; set; }
        public PackageRepositoryListDto Repositories { get; set; }
    }

    [XmlRoot("Repositories")]
    public class PackageRepositoryListDto : List<PackageRepositoryDto>, IXmlSerializable
    {    
        // http://stackoverflow.com/questions/15722978/i-cant-serialize-a-list-of-objects-in-c-sharp-with-xmlserializer

        #region IXmlSerializable
        public XmlSchema GetSchema(){ return null; }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("Repositories");
            while (reader.IsStartElement("DirectoryPackageRepository") || reader.IsStartElement("MultiPackageRepository"))
            {
                Type type;

                if (reader.LocalName == "DirectoryPackageRepository")
                {
                    type = typeof(DirectoryPackageRepositoryDto);
                }
                else if (reader.LocalName == "MultiPackageRepository")
                {
                    type = typeof(MultiPackageRepositoryDto);
                }
                else
                {
                    throw new Exception("PackageRepository Type is not supported for DeSerialization.");
                }

                XmlSerializer serial = new XmlSerializer(type);
                //reader.ReadStartElement(reader.LocalName);
                this.Add((PackageRepositoryDto)serial.Deserialize(reader));
                //reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (PackageRepositoryDto rep in this)
            {
                //writer.WriteStartElement("PackageRepository");
                //writer.WriteAttributeString("RepositoryType", rep.RepositoryType);
                XmlSerializer xmlSerializer = new XmlSerializer(rep.GetType());
                xmlSerializer.Serialize(writer, rep);
                //writer.WriteEndElement();
            }
        }
        #endregion
    }
}

