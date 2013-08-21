using System;
using System.Xml.Serialization;
using Waldi.Packages;
using AutoMapper;
using System.Xml;
using System.IO;
using Waldi.Engine;

namespace Waldi.Serialization
{
	public class PackageSerializer
	{
		private static string StreamToString(Stream stream)
		{
			stream.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(stream);
			return sr.ReadToEnd();
		}

		public static string Serialize(IPackage package)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				Serialize (package, stream);
				return StreamToString (stream);
			}
		}

		public static void Serialize(IPackage package, Stream stream)
		{
			Mapper.CreateMap<Feature, IPackage>();
			IPackage dto = Mapper.Map<IPackage>(package);

			XmlSerializer serializer = new XmlSerializer (typeof(PackageDto));
			XmlWriterSettings writerset = new XmlWriterSettings ();
			writerset.Indent = true;
			serializer.Serialize(XmlWriter.Create(stream, writerset), dto);
		}
	}
}

