using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using TestProject.DTO;
using TestProject.DTO.Device;
using TestProject.Models;

namespace TestProject.Services
{
    public class DeviceService : TestProjectAppServiceBase, IDeviceService
    {
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<DeviceType> _deviceTypeRepository;
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;
        private readonly IRepository<DevicePropertyValue> _valueRepository;

        private readonly IDeviceTypeService _deviceTypeService;

        public DeviceService(IRepository<Device> deviceRepository, IRepository<DeviceType> deviceTypeRepository, IRepository<DeviceTypeProperty> propertyRepository, IRepository<DevicePropertyValue> valueRepository, IDeviceTypeService deviceTypeService)
        {
            _deviceRepository = deviceRepository;
            _deviceTypeRepository = deviceTypeRepository;
            _propertyRepository = propertyRepository;
            _valueRepository = valueRepository;
            _deviceTypeService = deviceTypeService;
        }

        

        // ------------ GET DEVICE/DEVICES ---------------//

        public List<DeviceDto> GetAllDevices()
        {
            var devices = _deviceRepository.GetAll().Include(x => x.DeviceType).ToList();

            var result = ObjectMapper.Map<List<DeviceDto>>(devices);

            return result;
        }

        public List<DeviceTypeNestedDto> GetDeviceTypes()
        {
            var deviceTypes = _deviceTypeService.GetDeviceTypeNestedDtos(null);

            return deviceTypes;
        }

        public IEnumerable<DeviceTypePropertiesDto> GetDeviceTypePropertiesDtos(int id)
        {
            var deviceTypes = _deviceTypeService.GetdDeviceTypePropertiesDtos(id);

            return deviceTypes;
        }





    }
}
