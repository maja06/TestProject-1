using Abp.Application.Services.Dto;

namespace TestProject.DTO.DeviceTypeDtos
{
    public class DeviceTypePropertyDto : EntityDto
    {
        public string NameProperty { get; set; }

        public string Type { get; set; }

        public bool Required { get; set; }

        public int DeviceTypeId { get; set; }
    }
}