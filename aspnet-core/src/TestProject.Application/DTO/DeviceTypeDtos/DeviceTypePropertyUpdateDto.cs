using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace TestProject.DTO.DeviceTypeDtos
{
    public class DeviceTypePropertyUpdateDto : EntityDto
    {
        public List<DeviceTypePropertyDto> Properties { get; set; } = new List<DeviceTypePropertyDto>();
    }
}