using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestProject.Models
{
    public class DevicePropertyValue : Entity
    {
        public string Value { get; set; }

        public int DeviceTypePropertyId { get; set; }
        
        [ForeignKey("DeviceTypePropertyId")]
        public DeviceTypeProperty DeviceTypeProperty { get; set; }

        public int DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
        
    }
}
