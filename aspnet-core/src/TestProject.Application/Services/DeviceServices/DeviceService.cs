using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
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

        private readonly IRepository<DeviceType> _typeRepository;
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;
        private readonly IRepository<DevicePropertyValue> _valueRepository;

        public DeviceService(IRepository<Device> deviceRepository,
            IRepository<DeviceTypeProperty> propertyRepository, IRepository<DevicePropertyValue> valueRepository,
            IDeviceTypeService deviceTypeService, IRepository<DeviceType> typRepositoryRepository)
        {
            _deviceRepository = deviceRepository;
            _propertyRepository = propertyRepository;
            _valueRepository = valueRepository;
            _deviceTypeService = deviceTypeService;
            _typeRepository = typRepositoryRepository;
        }


        // ------------ GET DEVICE/DEVICES ---------------//

        public List<DeviceDto> GetDevices()
        {
            var devices = _deviceRepository.GetAll().Include(x => x.DeviceType).ToList();

            var result = ObjectMapper.Map<List<DeviceDto>>(devices);

            return result;
        }



        //----------------- DYNAMIC DEVICE DETAILS CONTAINING PORPERTIES ------------------//
        public List<IDictionary<string, object>> GetDevicesByType(int? id)
        {
            var deviceTypes = _deviceTypeService.GetDeviceTypeWithParents(id);

            List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();

            var allProperties = new List<DeviceTypeProperty>();

            foreach (var type in deviceTypes)
            {
                allProperties.AddRange(type.DeviceTypeProperties);
            }

            foreach (var type in deviceTypes)
            {
                foreach (var device in type.Devices)
                {
                    var values = device.DevicePropertyValues;

                    IDictionary<string, object> expando = new ExpandoObject();

                    expando.Add("Id", device.Id);
                    expando.Add("Name", device.Name);
                    expando.Add("Description", device.Description);

                    foreach (var prop in allProperties)
                    {
                        if (!device.DevicePropertyValues.Any())
                        {
                            string propName = prop.DeviceType.Name + "-" + prop.Name;

                            expando.Add(propName, null);
                        }

                        foreach (var value in values)
                        {

                            if (value.DeviceTypePropertyId == prop.Id)
                            {
                                string propName = prop.DeviceType.Name + "-" + prop.Name;

                                expando.Add(propName, value.Value);
                            }
                        }
                    }

                    result.Add(expando);
                }
            }

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


        // ------------------------- DELETE DEVICE ----------------------//

        public void DeleteDevice(int id)
        {
            var device = _deviceRepository.Get(id);

            _deviceRepository.Delete(device);
        }




    }
}