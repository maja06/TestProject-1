using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceDtos;

namespace TestProject.Services.DeviceServices
{
    public interface IDeviceService : IApplicationService
    {
        List<DeviceDto> GetDevices();

       void CreateOrUpdateDevice(CreateDeviceDto device);

        void DeleteDevice(int id);
    }
}