using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using TestProject.Models;

namespace TestProject.DTO
{
    public class DeviceTypePropertyUpdateDto : EntityDto
    {
        public List<DeviceTypePropertyDto> Properties { get; set; } = new List<DeviceTypePropertyDto>();
    }
}
