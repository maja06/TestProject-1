using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using TestProject.Models;

namespace TestProject.DTO
{
    public class DeviceTypeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

    }
}
