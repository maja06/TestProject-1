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
        public string nameProperty { get; set; }

        public string type { get; set; }
        
        public bool required { get; set; }
    }
}
