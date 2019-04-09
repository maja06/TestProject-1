using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace TestProject.Models
{
    public class DeviceTypeProperty : Entity
    {
        [StringLength(255)] [Required] public string Name { get; set; }

        public string Type { get; set; }

        public string MachineKey { get; set; }

        public int DeviceTypeId { get; set; }

        [ForeignKey("DeviceTypeId")] public DeviceType DeviceType { get; set; }

        public bool IsRequired { get; set; }
    }
}