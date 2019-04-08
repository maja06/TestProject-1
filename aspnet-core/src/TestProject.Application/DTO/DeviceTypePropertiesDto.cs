using System.Collections.Generic;
using Abp.Application.Services.Dto;
using TestProject.Models;

namespace TestProject.DTO
{
    public class DeviceTypePropertiesDto
    {
        public int Id { get; set; }
        
        public List<DeviceTypePropertyDto> Properties { get; set; } = new List<DeviceTypePropertyDto>();

    }
}
