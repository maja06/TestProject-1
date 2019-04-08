using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Application.Services.Dto;
using TestProject.Models;

namespace TestProject.DTO
{
    public class DeviceTypePropertyDto : EntityDto
    {
        public string NameProperty { get; set; }

        public string Type { get; set; }
        
        public bool Required { get; set; }
    }
}
