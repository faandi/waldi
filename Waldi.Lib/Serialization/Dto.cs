using System;
using System.Xml.Serialization;
using Waldi.Packages;
using AutoMapper;
using System.Xml;
using System.IO;
using Waldi.Engine;
using System.Collections.Generic;

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
    public class DirectoryPackageRepositoryDto
    {
        public string Name { get; set; }
        public string PackageDir { get; set; }
    }
}

