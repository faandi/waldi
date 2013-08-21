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

    public class PackageRepositoryListConverter : ITypeConverter<PackageRepositoryListDto, List<IPackageRepository>>
    {
        public List<IPackageRepository> Convert (ResolutionContext context)
        {
            PackageRepositoryListDto reps = context.SourceValue as PackageRepositoryListDto;

            List<IPackageRepository> result = new List<IPackageRepository>();

            foreach (PackageRepositoryDto r in reps)
            {
                IPackageRepository irep;
                if (r is DirectoryPackageRepositoryDto)
                {
                    irep = Mapper.Map<DirectoryPackageRepository>(r);
                }
                else if (r is MultiPackageRepositoryDto)
                {
                    irep = Mapper.Map<MultiPackageRepository>(r);
                }
                else
                {
                    throw new Exception("Type not supported.");
                }
                result.Add(irep);
            }
            return result;
        }
    }

    public class PackageRepositoryListConverterDto : ITypeConverter<List<IPackageRepository>, PackageRepositoryListDto>
    {
        public PackageRepositoryListDto Convert (ResolutionContext context)
        {
            PackageRepositoryListDto result = new PackageRepositoryListDto();
            List<IPackageRepository> reps = context.SourceValue as List<IPackageRepository>;
            foreach (IPackageRepository r in reps)
            {
                PackageRepositoryDto dtor;
                if (r is DirectoryPackageRepository)
                {
                    dtor = Mapper.Map<DirectoryPackageRepositoryDto>(r);
                }
                else if (r is MultiPackageRepository)
                {
                    dtor = Mapper.Map<MultiPackageRepositoryDto>(r);
                }
                else
                {
                    throw new Exception("Type not supported.");
                }
                result.Add(dtor);
            }
            return result;
        }
    }

    public class PackageRepositoryConverter : ITypeConverter<PackageRepositoryDto, IPackageRepository>
    {
        public IPackageRepository Convert (ResolutionContext context)
        {
            PackageRepositoryDto dto = context.SourceValue as PackageRepositoryDto;
            if (dto is MultiPackageRepositoryDto)
            {
                return Mapper.Map<MultiPackageRepository>(dto);
            }
            else if (dto is DirectoryPackageRepositoryDto)
            {
                return Mapper.Map<DirectoryPackageRepository>(dto);
            }
            else
            {
                throw new Exception("Type not supported.");
            }
        }
    }


    /*
    public class PackageRepositoryConverterDto : ITypeConverter<IPackageRepository, PackageRepositoryDto>
    {
        public PackageRepositoryDto Convert (ResolutionContext context)
        {
            IPackageRepository rep = context.SourceValue as IPackageRepository;

            if (rep is DirectoryPackageRepository)
            {
                DirectoryPackageRepository typedrep = rep as DirectoryPackageRepository;
                Uri pathuri = new Uri(typedrep.PackageDir.FullName);
                return new DirectoryPackageRepositoryDto()
                {
                    Name = typedrep.Name,
                    PackageDir = pathuri.AbsoluteUri,
                    Type =  "directory"
                };
            }
            throw new Exception("Type is not supported.");
        }
    }
    */

    /*
    public class MultiPackageRepositoryConverterDto : ITypeConverter<MultiPackageRepository, MultiPackageRepositoryDto>
    {
        public MultiPackageRepositoryDto Convert (ResolutionContext context)
        {
            MultiPackageRepository rep = context.SourceValue as MultiPackageRepository;
            //Uri pathuri = new Uri(rep.PackageDir.FullName);

            List<PackageRepositoryDto> repositories = new List<PackageRepositoryDto>();
            foreach (IPackageRepository v in rep.Repositories)
            {
                if (v is DirectoryPackageRepository)
                {
                    repositories.Add(Mapper.Map<DirectoryPackageRepositoryDto>(v));
                }
                else if (v is MultiPackageRepository)
                {
                    repositories.Add(Mapper.Map<MultiPackageRepositoryDto>(v));
                }
                else
                {

                }
            }

            return new MultiPackageRepositoryDto()
            {
                Name = rep.Name,
                Repositories = repositories,
                Type =  "multi"
            };
        }
    }
    */
}
