using System;
using System.Xml.Serialization;
using Waldi.Packages;
using AutoMapper;
using System.Xml;
using System.IO;

namespace Waldi.Serialization
{
	public class FeatureSerializer
	{

		// TODO: Basis Serializer Klasse
		// oder doch nur eine Klasse für alles ? 
		private static string StreamToString(Stream stream)
		{
			stream.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(stream);
			return sr.ReadToEnd();
		}

		public static string Serialize(Feature feature)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				Serialize (feature, stream);
				return StreamToString (stream);
			}
		}

		public static void Serialize(Feature feature, Stream stream)
		{
			Mapper.CreateMap<Feature, FeatureDto>();
			FeatureDto dto = Mapper.Map<FeatureDto>(feature);

			XmlSerializer serializer = new XmlSerializer (typeof(FeatureDto));
			XmlWriterSettings writerset = new XmlWriterSettings ();
			writerset.Indent = true;
			serializer.Serialize(XmlWriter.Create(stream, writerset), dto);
		}
	}
}

