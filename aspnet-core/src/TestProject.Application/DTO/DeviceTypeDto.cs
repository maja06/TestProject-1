﻿using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using TestProject.Models;

namespace TestProject.DTO
{
    public class DeviceTypeDto : EntityDto
    {
        public string name { get; set; }

        public string description { get; set; }

        public int? parentid { get; set; }

        public List<DeviceTypePropertyDto> properties { get; set; } = new List<DeviceTypePropertyDto>();
    }
}
