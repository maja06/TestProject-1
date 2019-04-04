using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestProject.Models
{
    public class Device : Entity
    {
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public int DeviceTypeId { get; set; }

        [ForeignKey("DeviceTypeId")]
        public DeviceType DeviceType { get; set; }
        
        public List<DevicePropertyValue> DevicePropertyValues { get; set; } = new List<DevicePropertyValue>();
    }
}
