using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceDtos;
using TestProject.DTO.DeviceTypeDtos;

namespace TestProject.Services.DeviceServices
{
    public interface IDeviceService : IApplicationService
    {
        List<DeviceDto> GetDevices();

        List<IDictionary<string, object>> GetDevicesByType(int? id);
        
        void CreateOrUpdateDevice(CreateDeviceDto device);

        void DeleteDevice(int id);
    }
}