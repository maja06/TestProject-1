using Abp.Application.Services.Dto;

namespace TestProject.DTO.Device
{
    public class DeviceDto : EntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string DeviceTypeName { get; set; }

    }
}
