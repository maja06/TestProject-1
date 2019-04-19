using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceTypeDtos;
using TestProject.Models;

namespace TestProject.Services.DeviceTypeServices
{
    public interface IDeviceTypeService : IApplicationService
    {
        List<DeviceTypeNestedDto> GetDeviceTypes(int? parentId);

        IEnumerable<DeviceTypePropertiesDto> GetDeviceTypesWithProperties(int? deviceTypeId);

        IEnumerable<DeviceType> GetDeviceTypeWithChildren(int parentId);

        IEnumerable<DeviceType> GetDeviceTypeWithParents(int? id);

        List<IDictionary<string, object>> GetDevicesByType(int? id);

        void CreateOrUpdateDeviceType(DeviceTypePropertiesDto input);
        
        void DeleteDeviceType(int id);
    }
}