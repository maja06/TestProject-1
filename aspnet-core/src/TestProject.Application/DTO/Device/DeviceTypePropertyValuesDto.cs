﻿using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace TestProject.DTO.Device
{
    public class DeviceTypePropertyValuesDto : EntityDto
    {
        public List<PropertyValueDto> PropValues { get; set; } = new List<PropertyValueDto>();

    }
}
