using System.Collections.Generic;
using Abp.Application.Services;
using TestProject.DTO.DeviceDtos;
using TestProject.Query;

namespace TestProject.Services.DeviceServices
{
    public interface IDeviceService : IApplicationService
    {
        List<DeviceDto> GetDevices();

        void CreateOrUpdateDevice(CreateDeviceDto device);

        void DeleteDevice(int id);

        List<DeviceDto> GetSearchResult(QueryInfo query);

        UpdateDeviceDto GetDeviceTypesWithPropValues(int deviceId, int deviceTypeId);

    }
}