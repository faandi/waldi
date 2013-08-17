using System;
using System.Xml.Serialization;
using Waldi.Packages;
using AutoMapper;
using System.Xml;
using System.IO;
using Waldi.Engine;
using System.Linq;
using System.Collections.Generic;
using Waldi.Repositories;

namespace Waldi.Serialization
{
    public static class WaldiSerializer
    {
        public static void MapType(Type objtype, Type serobjtype)
        {
            Mapper.CreateMap(objtype, serobjtype);
        }

        public static string Serialize(object obj, Type objtype, Type serobjtype)
        {
            return WaldiSerializerInternal.Serialize(obj, objtype, serobjtype);
        }

        public static void Serialize(object obj, Stream stream, Type objtype, Type serobjtype)
        {
            WaldiSerializerInternal.Serialize(obj, objtype, serobjtype, stream);
        }

        public static string Serialize(Feature obj)
        {
            //return WaldiSerializer<Feature,FeatureDto>.Serialize(obj);
            return WaldiSerializerInternal.Serialize(obj, typeof(Feature), typeof(FeatureDto));
        }

        public static void Serialize(Feature obj, Stream stream)
        {
            //WaldiSerializer<Feature,FeatureDto>.Serialize(obj, stream);
            WaldiSerializerInternal.Serialize(obj, typeof(Feature), typeof(FeatureDto), stream);
        }

        public static string Serialize(IPackage obj)
        {
            //return WaldiSerializer<IPackage,PackageDto>.Serialize(obj);
            return WaldiSerializerInternal.Serialize(obj, typeof(IPackage), typeof(PackageDto));
        }

        public static void Serialize(IPackage obj, Stream stream)
        {
            //WaldiSerializer<IPackage,PackageDto>.Serialize(obj, stream);
            WaldiSerializerInternal.Serialize(obj, typeof(IPackage), typeof(PackageDto), stream);
        }

        public static string Serialize(IPackageRepository obj)
        {
            if (obj is DirectoryPackageRepository)
            {
                //return WaldiSerializer<DirectoryPackageRepository,DirectoryPackageRepositoryDto>.Serialize(obj as DirectoryPackageRepository);
                return WaldiSerializerInternal.Serialize(obj, typeof(DirectoryPackageRepository), typeof(DirectoryPackageRepositoryDto));
            }
            throw new ArgumentException("Type is not supported for serialization.", "obj");
        }

        public static void Serialize(IPackageRepository obj, Stream stream)
        {
            if (obj is DirectoryPackageRepository)
            {
                //WaldiSerializer<DirectoryPackageRepository,DirectoryPackageRepositoryDto>.Serialize(obj as DirectoryPackageRepository, stream);
                WaldiSerializerInternal.Serialize(obj, typeof(DirectoryPackageRepository), typeof(DirectoryPackageRepositoryDto), stream);
            }
            throw new ArgumentException("Type is not supported for serialization.", "obj");
        }

        public static object Deserialize(string objstr, Type objtype, Type serobjtype)
        {
            return WaldiSerializerInternal.Deserialize(objstr, objtype, serobjtype);
        }

        public static object Deserialize(StreamReader stream, Type objtype, Type serobjtype)
        {
            return WaldiSerializerInternal.Deserialize(stream, objtype, serobjtype);
        }

        public static Feature DeserializeFeature(string objstr)
        {
            //return WaldiSerializer<Feature,FeatureDto>.Deserialize(objstr);
            return WaldiSerializerInternal.Deserialize(objstr, typeof(Feature), typeof(FeatureDto)) as Feature;
        }

        public static Feature DeserializeFeature(StreamReader stream)
        {
            //return WaldiSerializer<Feature,FeatureDto>.Deserialize(stream);
            return WaldiSerializerInternal.Deserialize(stream, typeof(Feature), typeof(FeatureDto)) as Feature;
        }

        public static IPackage DeserializePackage(string objstr)
        {
            //return WaldiSerializer<BasicPackage,PackageDto>.Deserialize(objstr);
            return WaldiSerializerInternal.Deserialize(objstr, typeof(BasicPackage), typeof(PackageDto)) as IPackage;
        }

        public static IPackage DeserializePackage(StreamReader stream)
        {
            //return WaldiSerializer<BasicPackage,PackageDto>.Deserialize(stream);
            return WaldiSerializerInternal.Deserialize(stream, typeof(BasicPackage), typeof(PackageDto)) as IPackage;
        }

        public static IPackageRepository DeserializePackageRepository(string objstr)
        {
            //return WaldiSerializer<DirectoryPackageRepository,DirectoryPackageRepositoryDto>.Deserialize(objstr);
            return WaldiSerializerInternal.Deserialize(objstr, typeof(DirectoryPackageRepository), typeof(DirectoryPackageRepositoryDto)) as IPackageRepository;
        }

        public static IPackageRepository DeserializePackageRepository(StreamReader stream)
        {
            //return WaldiSerializer<DirectoryPackageRepository,DirectoryPackageRepositoryDto>.Deserialize(stream);
            return WaldiSerializerInternal.Deserialize(stream, typeof(DirectoryPackageRepository), typeof(DirectoryPackageRepositoryDto)) as IPackageRepository;
        }
    }

    // TODO: <TObj,TSerObj> in die Funktions Definitionen auslagern
    internal static class WaldiSerializerInternal
        //<TObj,TSerObj>
        //where TObj : class
        //where TSerObj : class
    {
        private static bool isInitialized = false;

        private static void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            //Mapper.CreateMap<TObj, TSer> ();

            Mapper.CreateMap<List<PackageVersionDto>, NamedItemList<PackageVersion>>().ConvertUsing(new NamedItemListPackageVersionConverter());
            Mapper.CreateMap<List<DependencyDto>, DependencyList>().ConvertUsing(new DependencyListConverter());
            Mapper.CreateMap<List<FeatureDto>, FeatureList>().ConvertUsing(new FeatureListConverter());
            Mapper.CreateMap<DirectoryPackageRepositoryDto, DirectoryPackageRepository>().ConvertUsing(new DirectoryPackageRepositoryConverter());

            Mapper.CreateMap<Dependency, DependencyDto>();
            Mapper.CreateMap<PackageVersion, PackageVersionDto>();
            Mapper.CreateMap<IPackage, PackageDto>();
            Mapper.CreateMap<Feature, FeatureDto>();		
            Mapper.CreateMap<DirectoryPackageRepository, DirectoryPackageRepositoryDto>().ConvertUsing(new DirectoryPackageRepositoryConverterDto());

            Mapper.CreateMap<DependencyDto, Dependency>()
				.ForMember(x => x.PackageName, opt => opt.Ignore())
				.ForMember(x => x.FeatureName, opt => opt.Ignore());
            Mapper.CreateMap<PackageVersionDto, PackageVersion>()
				.ForMember(x => x.Qualifier, opt => opt.Ignore())
				.ForMember(x => x.Number, opt => opt.Ignore());
            Mapper.CreateMap<PackageDto, BasicPackage>()
				.ForMember(x => x.Packages, opt => opt.Ignore());
            Mapper.CreateMap<FeatureDto, Feature>();

            Mapper.AssertConfigurationIsValid();
            isInitialized = true;
        }

        private static string StreamToString(Stream stream)
        {
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        /*
        public static string Serialize(TObj obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serialize(obj, stream);
                return StreamToString(stream);
            }
        }

        public static void Serialize(TObj obj, Stream stream)
        {			
            Initialize();
            TSerObj dto = Mapper.Map<TSerObj>(obj);

            XmlSerializer serializer = new XmlSerializer(typeof(TSerObj));
            XmlWriterSettings writerset = new XmlWriterSettings();
            writerset.Indent = true;
            serializer.Serialize(XmlWriter.Create(stream, writerset), dto);
        }
        */

        public static string Serialize(object obj, Type objtype, Type serobjtype)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serialize(obj, objtype, serobjtype, stream);
                return StreamToString(stream);
            }
        }

        public static void Serialize(object obj, Type objtype, Type serobjtype, Stream stream)
        {           
            Initialize();
            object dto = Mapper.Map(obj, objtype, serobjtype);

            XmlSerializer serializer = new XmlSerializer(serobjtype);
            XmlWriterSettings writerset = new XmlWriterSettings();
            writerset.Indent = true;
            serializer.Serialize(XmlWriter.Create(stream, writerset), dto);
        }

        /*
        public static TObj Deserialize(string objstr)
        {
            Initialize();
            XmlSerializer s = new XmlSerializer(typeof(TSerObj));
            using (TextReader reader = new StringReader(objstr))
            {
                TSerObj objser = s.Deserialize(reader) as TSerObj;
                return Mapper.Map<TObj>(objser);
            }
        }

        public static TObj Deserialize(StreamReader stream)
        {			
            Initialize();
            XmlSerializer s = new XmlSerializer(typeof(TSerObj));
            TSerObj objser = s.Deserialize(XmlReader.Create(stream)) as TSerObj;
            return Mapper.Map<TObj>(objser);
        }
        */

        public static object Deserialize(string objstr, Type objtype, Type serobjtype)
        {
            Initialize();
            XmlSerializer s = new XmlSerializer(serobjtype);
            using (TextReader reader = new StringReader(objstr))
            {
                object objser = s.Deserialize(reader);
                return Mapper.Map(objser, serobjtype, objtype);
            }
        }

        public static object Deserialize(StreamReader stream, Type objtype, Type serobjtype)
        {           
            Initialize();
            XmlSerializer s = new XmlSerializer(serobjtype);
            object objser = s.Deserialize(XmlReader.Create(stream));
            return Mapper.Map(objser, serobjtype, objtype);
        }
    }
}