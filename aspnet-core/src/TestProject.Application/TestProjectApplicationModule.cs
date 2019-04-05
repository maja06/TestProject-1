using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TestProject.Authorization;
using TestProject.DTO;
using TestProject.Models;

namespace TestProject
{
    [DependsOn(
        typeof(TestProjectCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class TestProjectApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<TestProjectAuthorizationProvider>();




        }

        public override void Initialize()
        {
            var thisAssembly = typeof(TestProjectApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg =>
                {
                    cfg.AddProfiles(thisAssembly);

                    cfg.CreateMap<DeviceType, DeviceTypeDto>()
                        .ForMember(dest => dest.name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.parentid, source => source.MapFrom(src => src.ParentDeviceType.Id))
                        .ForMember(dest => dest.properties, source => source.MapFrom(src => src.DeviceTypeProperties));

                    cfg.CreateMap<DeviceType, DeviceTypeForListDto>()
                        .ForMember(dest => dest.name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.parentid, source => source.MapFrom(src => src.ParentDeviceType.Id));

                    cfg.CreateMap<DeviceType, DeviceTypeNestedDto>()
                        .ForMember(dest => dest.name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.parentid, source => source.MapFrom(src => src.ParentDeviceType.Id))
                        .ForMember(dest => dest.children, source => source.Ignore());

                    cfg.CreateMap<DeviceTypeProperty, DeviceTypePropertyDto>()
                        .ForMember(dest => dest.nameProperty, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.required, source => source.MapFrom(src => src.isRequired))
                        .ForMember(dest => dest.type, source => source.MapFrom(src => src.Type));
                }
            );
        }
    }
}
