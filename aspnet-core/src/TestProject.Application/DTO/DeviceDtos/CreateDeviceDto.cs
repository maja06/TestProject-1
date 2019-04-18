using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace TestProject.DTO.DeviceDtos
{
    public class CreateDeviceDto : EntityDto
    {
        public string DeviceName { get; set; }

        public string Description { get; set; }

        public int DeviceTypeId { get; set; }

        public List<DeviceTypePropertyValuesDto> DeviceTypes { get; set; } = new List<DeviceTypePropertyValuesDto>();
    }
}