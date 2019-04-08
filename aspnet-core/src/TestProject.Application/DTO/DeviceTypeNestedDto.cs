using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace TestProject.DTO
{
    public class DeviceTypeNestedDto : EntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        public List<DeviceTypeNestedDto> Children { get; set; } = new List<DeviceTypeNestedDto>();
    }
}
