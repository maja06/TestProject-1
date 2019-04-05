using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Application.Services.Dto;
using TestProject.Models;

namespace TestProject.DTO
{
    public class DeviceTypeNestedDto : EntityDto
    {
        public string name { get; set; }

        public string description { get; set; }

        public int? parentid { get; set; }

        public List<DeviceTypeNestedDto> children { get; set; } = new List<DeviceTypeNestedDto>();
    }
}
