using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestProject.Models
{
    public class DeviceType : Entity
    {
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public int? ParentDeviceTypeId { get; set; }

        [ForeignKey("ParentDeviceTypeId")]
        [CanBeNull] public DeviceType ParentDeviceType { get; set; }

        public List<Device> Devices { get; set; } = new List<Device>();

        public List<DeviceTypeProperty> DeviceTypeProperties { get; set; } = new List<DeviceTypeProperty>();

    }
}
