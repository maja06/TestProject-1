using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceTypeDtos;
using TestProject.Models;

namespace TestProject.Services.DeviceTypeServices
{
    public interface IDeviceTypeService : IApplicationService
    {
        IEnumerable<DeviceTypePropertiesDto> GetDeviceTypesFlatList(int? deviceTypeId);

        List<DeviceTypeNestedDto> GetDeviceTypesNestedList(int? parentId);

        IEnumerable<DeviceType> GetDeviceTypeWithChildren(int parentId);

        IEnumerable<DeviceTypePropertiesDto> CreateOrUpdateDeviceType(DeviceTypeDto deviceTypeDto);

        void UpdateDeviceTypeProperties(DeviceTypePropertyUpdateDto deviceTypePropertyUpdateDto);

        void Delete(int id);

        IEnumerable<DeviceType> GetDeviceTypeWithParents(int? id);






    }
}