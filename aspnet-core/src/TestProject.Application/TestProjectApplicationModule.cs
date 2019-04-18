using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TestProject.Authorization;
using TestProject.DTO.DeviceDtos;
using TestProject.DTO.DeviceTypeDtos;
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


                    // ----------------- CUSTOM MAPPINGS -----------------//

                    // ------------------- DEVICE TYPE ------------------//
                    cfg.CreateMap<DeviceType, DeviceTypeDto>()
                        .ForMember(dest => dest.Name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.Description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.ParentId, source => source.MapFrom(src => src.ParentDeviceType.Id));

                    cfg.CreateMap<DeviceTypeDto, DeviceType>()
                        .ForMember(dest => dest.Name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.Description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.ParentDeviceTypeId, source => source.MapFrom(src => src.ParentId))
                        .ForMember(dest => dest.ParentDeviceType, source => source.Ignore());


                    cfg.CreateMap<DeviceType, DeviceTypeNestedDto>()
                        .ForMember(dest => dest.Name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.Description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.ParentId, source => source.MapFrom(src => src.ParentDeviceType.Id))
                        .ForMember(dest => dest.Items, source => source.Ignore());

                    cfg.CreateMap<DeviceType, DeviceTypePropertiesDto>()
                        .ForMember(dest => dest.Name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.Description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.ParentId, source => source.MapFrom(src => src.ParentDeviceType.Id))
                        .ForMember(dest => dest.Properties, source => source.MapFrom(src => src.DeviceTypeProperties));


                    //--------------- TYPE PROPERTY -----------------//
                    cfg.CreateMap<DeviceTypeProperty, DeviceTypePropertyDto>()
                        .ForMember(dest => dest.NameProperty, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.Required, source => source.MapFrom(src => src.IsRequired))
                        .ForMember(dest => dest.Type, source => source.MapFrom(src => src.Type))
                        .ForMember(dest => dest.DeviceTypeId, source => source.MapFrom(src => src.DeviceTypeId));


                    // ------------------- DEVICE ------------------//
                    cfg.CreateMap<Device, DeviceDto>()
                        .ForMember(dest => dest.Name, source => source.MapFrom(src => src.Name))
                        .ForMember(dest => dest.Description, source => source.MapFrom(src => src.Description))
                        .ForMember(dest => dest.DeviceTypeName, source => source.MapFrom(src => src.DeviceType.Name))
                        .ForMember(dest => dest.DeviceTypeId, source => source.MapFrom(src => src.DeviceTypeId));


                }
            );
        }
    }
}