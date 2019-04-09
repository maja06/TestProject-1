using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceTypeDtos;

namespace TestProject.Services.DeviceTypeServices
{
    public interface IDeviceTypeService : IApplicationService
    {
        IEnumerable<DeviceTypePropertiesDto> GetdDeviceTypePropertiesDtos(int? deviceTypeId);

        List<DeviceTypeNestedDto> GetDeviceTypeNestedDtos(int? parentId);

        IEnumerable<DeviceTypePropertiesDto> CreateOrUpdateDeviceType(DeviceTypeDto deviceTypeDto);

        void UpdateDeviceTypeProperties(DeviceTypePropertyUpdateDto deviceTypePropertyUpdateDto);
    }
}