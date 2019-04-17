using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.DTO.DeviceDtos;
using TestProject.Models;
using TestProject.Query;

namespace TestProject.Services.DeviceServices
{
    public class DeviceService : TestProjectAppServiceBase, IDeviceService
    {
        private readonly IRepository<Device> _deviceRepository;
        
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;
        private readonly IRepository<DevicePropertyValue> _valueRepository;

        public DeviceService(IRepository<Device> deviceRepository,
            IRepository<DeviceTypeProperty> propertyRepository, IRepository<DevicePropertyValue> valueRepository)
        {
            _deviceRepository = deviceRepository;
            _propertyRepository = propertyRepository;
            _valueRepository = valueRepository;
        }


        // ------------ GET DEVICE/DEVICES ---------------//

        public List<DeviceDto> GetDevices()
        {
            var devices = _deviceRepository.GetAll().Include(x => x.DeviceType).ToList();

            var result = ObjectMapper.Map<List<DeviceDto>>(devices);

            return result;
        }



        // ---------------------------- CREATE DEVICE -----------------------------//
        // -------------------------------- STEP 3 --------------------------------//
        public void CreateOrUpdateDevice(CreateDeviceDto device)
        {
            if (device.Id == 0)
            {
                var newDevice = new Device
                {
                    Name = device.DeviceName,
                    Description = device.Description
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
                .First(x => x.Id == device.Id);

            targetDevice.Name = device.DeviceName;
            targetDevice.Description = device.Description;

            foreach (var deviceType in device.DeviceTypes)
            {
                var propValues = deviceType.PropValues;

                foreach (var propValue in propValues)
                {
                    var values = _valueRepository.GetAll().Include(x => x.Device).Include(x => x.DeviceTypeProperty);

                    var value = values.First(x =>
                        x.DeviceId == targetDevice.Id && x.DeviceTypeProperty.Name == propValue.PropName);

                    value.Value = propValue.Value;
                }
            }
        }


        // ------------------------- DELETE DEVICE ----------------------//

        public void DeleteDevice(int id)
        {
            var device = _deviceRepository.Get(id);

            _deviceRepository.Delete(device);
        }


        // --------------------- SEARCH QUERY-----------------------------//
        [HttpPost]
        public List<DeviceDto> GetSearchResult([FromBody]QueryInfo query)
        {
            var allDevices = _deviceRepository.GetAll().Include(x => x.DeviceType);
            
            var result = query.GetQuery(query, allDevices).ToList();

            var dtoResult = ObjectMapper.Map<List<DeviceDto>>(result);

            return dtoResult;
        }


    }
}