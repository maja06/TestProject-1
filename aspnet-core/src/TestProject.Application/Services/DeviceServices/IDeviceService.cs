using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceDtos;
using TestProject.Models;
using TestProject.Query;

namespace TestProject.Services.DeviceServices
{
    public interface IDeviceService : IApplicationService
    {
        List<DeviceDto> GetDevices();

       void CreateOrUpdateDevice(CreateDeviceDto device);

        void DeleteDevice(int id);

        List<Device> GetSearchResult(QueryInfo query);

    }
}