using System.Collections.Generic;
using Abp.Application.Services.Dto;
using TestProject.Models;

namespace TestProject.DTO
{
    public class DeviceTypeForListDto : EntityDto
    {
        public string name { get; set; }

        public string description { get; set; }

        public int? parentid { get; set; }
    }
}
