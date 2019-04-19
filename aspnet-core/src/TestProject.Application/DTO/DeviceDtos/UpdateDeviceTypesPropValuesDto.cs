using System.Collections.Generic;

namespace TestProject.DTO.DeviceDtos
{
    public class UpdateDeviceTypesPropValuesDto
    {
        public int DeviceTypeId { get; set; }

        public string DeviceTypeName { get; set; }

        public List<UpdateDevicePropValueDto> PropValues { get; set; } = new List<UpdateDevicePropValueDto>();
    }
}
