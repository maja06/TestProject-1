using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace TestProject.DTO.Device
{
    public class CreateDeviceDto : EntityDto
    {
        public string DeviceName { get; set; }

        public string Description { get; set; }

        public List<DeviceTypePropertyValuesDto> DeviceTypes { get; set; } = new List<DeviceTypePropertyValuesDto>();
    }
}
