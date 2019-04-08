using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;

namespace TestProject.DTO.Device
{
    public class DeviceDto : EntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string DeviceTypeName { get; set; }

    }
}
