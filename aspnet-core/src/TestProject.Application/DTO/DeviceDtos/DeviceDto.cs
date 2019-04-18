using Abp.Application.Services.Dto;

namespace TestProject.DTO.DeviceDtos
{
    public class DeviceDto : EntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string DeviceTypeName { get; set; }

        public int DeviceTypeId { get; set; }
    }
}