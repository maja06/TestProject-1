using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestProject.Models
{
    public class DeviceTypeProperty : Entity
    {
        [StringLength(255)]
        [Required]
        public string Name { get; set; }
        
        public string MachineKey { get; set; }

        public int DeviceTypeId { get; set; }

        [ForeignKey("DeviceTypeId")]
        public DeviceType DeviceType { get; set; }

        public bool isRequired { get; set; }
    }
}
