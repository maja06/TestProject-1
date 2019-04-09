using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using TestProject.DTO.DeviceDtos;
using TestProject.DTO.DeviceTypeDtos;
using TestProject.Models;
using TestProject.Services.DeviceTypeServices;

namespace TestProject.Services.DeviceServices
{
    public class DeviceService : TestProjectAppServiceBase, IDeviceService
    {
        private readonly IRepository<Device> _deviceRepository;

        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;
        private readonly IRepository<DevicePropertyValue> _valueRepository;

        public DeviceService(IRepository<Device> deviceRepository,
            IRepository<DeviceTypeProperty> propertyRepository, IRepository<DevicePropertyValue> valueRepository,
            IDeviceTypeService deviceTypeService)
        {
            _deviceRepository = deviceRepository;
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


        // --------------- CREATE DEVICE -----------------//

        public IEnumerable<DeviceTypePropertiesDto> GetDeviceTypeListDtos(int id)
        {
            var deviceTypes = _deviceTypeService.GetdDeviceTypePropertiesDtos(id);

            return deviceTypes;
        }


        public void CreateOrUpdateDevice(CreateDeviceDto device)
        {
            if (device.Id == 0)
            {
                var newDevice = new Models.Device
                {
                    Name = device.DeviceName,
                    Description = device.Description,
                    DevicePropertyValues = new List<DevicePropertyValue>()
                };

                var valueList = new List<DevicePropertyValue>();

                var maxDeviceTypeId = device.DeviceTypes.Max(x => x.Id);

                foreach (var deviceType in device.DeviceTypes)
                {
                    var propValues = deviceType.PropValues;

                    foreach (var propValue in propValues)
                        valueList.Add(new DevicePropertyValue
                        {
                            Value = propValue.Value,
                            DeviceTypePropertyId = _propertyRepository.FirstOrDefault(x =>
                                x.DeviceTypeId == deviceType.Id && x.Name == propValue.PropName).Id,
                            DeviceId = newDevice.Id
                        });
                }

                newDevice.DeviceTypeId = maxDeviceTypeId;
                newDevice.DevicePropertyValues = valueList;

                _deviceRepository.Insert(newDevice);

                return;
            }

            var targetDevice = _deviceRepository.GetAll().Include(x => x.DevicePropertyValues)
                .FirstOrDefault(x => x.Id == device.Id);

            targetDevice.Name = device.DeviceName;
            targetDevice.Description = device.Description;

            foreach (var deviceType in device.DeviceTypes)
            {
                var propValues = deviceType.PropValues;

                foreach (var propValue in propValues)
                {
                    var values = _valueRepository.GetAll().Include(x => x.Device).Include(x => x.DeviceTypeProperty);

                    var value = values.FirstOrDefault(x =>
                        x.DeviceId == targetDevice.Id && x.DeviceTypeProperty.Name == propValue.PropName);

                    value.Value = propValue.Value;
                }
            }
        }


        public void Delete(int id)
        {
            var device = _deviceRepository.Get(id);

            _deviceRepository.Delete(device);
        }
    }
}