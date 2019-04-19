using System.Collections.Generic;

namespace TestProject.DTO.DeviceDtos
{
    public class UpdateDeviceDto
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string Description { get; set; }

        public int TypeId { get; set; }

        public List<UpdateDeviceTypesPropValuesDto> DeviceTypes { get; set; } = new List<UpdateDeviceTypesPropValuesDto>();
    }
}
