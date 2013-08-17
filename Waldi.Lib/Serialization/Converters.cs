using System;
using Waldi.Engine;
using AutoMapper;
using System.Collections.Generic;
using Waldi.Packages;
using Waldi.Repositories;
using Waldi.BclExtensions;
using System.IO;

namespace Waldi.Serialization
{
	public class NamedItemListPackageVersionConverter : ITypeConverter<List<PackageVersionDto>, NamedItemList<PackageVersion>>
	{
		public NamedItemList<PackageVersion> Convert (ResolutionContext context)
		{
			List<PackageVersionDto> list = context.SourceValue as List<PackageVersionDto>;
			NamedItemList<PackageVersion> namedlist = new NamedItemList<PackageVersion> ();
			foreach (PackageVersionDto v in list)
			{
				namedlist.Add (Mapper.Map<PackageVersion> (v));
			}
			return namedlist;
		}
	}

	public class DependencyListConverter : ITypeConverter<List<DependencyDto>, DependencyList>
	{
		public DependencyList Convert (ResolutionContext context)
		{
			List<DependencyDto> list = context.SourceValue as List<DependencyDto>;
			DependencyList namedlist = new DependencyList ();
			foreach (DependencyDto v in list)
			{
				namedlist.Add (Mapper.Map<Dependency> (v));
			}
			return namedlist;
		}
	}

	public class FeatureListConverter : ITypeConverter<List<FeatureDto>, FeatureList>
	{
		public FeatureList Convert (ResolutionContext context)
		{
			List<FeatureDto> list = context.SourceValue as List<FeatureDto>;
			FeatureList namedlist = new FeatureList ();
			foreach (FeatureDto v in list)
			{
				namedlist.Add (Mapper.Map<Feature> (v));
			}
			return namedlist;
		}
	}

    public class DirectoryPackageRepositoryConverter : ITypeConverter<DirectoryPackageRepositoryDto, DirectoryPackageRepository>
    {
        public DirectoryPackageRepository Convert (ResolutionContext context)
        {
            DirectoryPackageRepositoryDto dto = context.SourceValue as DirectoryPackageRepositoryDto;
            Uri workinguri = new Uri(Directory.GetCurrentDirectory());
            Uri pathuri = UriExtensions.GetFileUri(dto.PackageDir, workinguri.AbsolutePath);
            return new DirectoryPackageRepository(dto.Name, pathuri.LocalPath);
        }
    }

    public class DirectoryPackageRepositoryConverterDto : ITypeConverter<DirectoryPackageRepository, DirectoryPackageRepositoryDto>
    {
        public DirectoryPackageRepositoryDto Convert (ResolutionContext context)
        {
            DirectoryPackageRepository rep = context.SourceValue as DirectoryPackageRepository;
            Uri pathuri = new Uri(rep.PackageDir.FullName);
            return new DirectoryPackageRepositoryDto()
            {
                Name = rep.Name,
                PackageDir = pathuri.AbsoluteUri
            };
        }
    }
}
