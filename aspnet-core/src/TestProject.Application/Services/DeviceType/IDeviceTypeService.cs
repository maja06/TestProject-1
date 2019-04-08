using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services;
using TestProject.DTO;
using TestProject.Models;

namespace TestProject.Services
{
    public interface IDeviceTypeService : IApplicationService
    {
        IEnumerable<DeviceTypePropertiesDto> GetdDeviceTypePropertiesDtos(int? deviceTypeId);

        List<DeviceTypeNestedDto> GetDeviceTypeNestedDtos(int? parentId);

        DeviceType GetDeviceTypeById(int id);
        
        IEnumerable<DeviceTypePropertiesDto> CreateOrUpdateDeviceType(DeviceTypeDto deviceTypeDto);

        void UpdateDeviceTypeProperties(DeviceTypePropertyUpdateDto deviceTypePropertyUpdateDto);
        
    }
}
