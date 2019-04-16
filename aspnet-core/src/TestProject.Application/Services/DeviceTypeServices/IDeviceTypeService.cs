using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceTypeDtos;

namespace TestProject.Services.DeviceTypeServices
{
    public interface IDeviceTypeService : IApplicationService
    {
        List<DeviceTypeNestedDto> GetDeviceTypes(int? parentId);

        IEnumerable<DeviceTypePropertiesDto> GetDeviceTypesWithProperties(int? deviceTypeId);

        List<IDictionary<string, object>> GetDevicesByType(int? id);

        void CreateOrUpdateDeviceType(DeviceTypePropertiesDto input);
        
        void DeleteDeviceType(int id);
    }
}