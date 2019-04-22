using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.DTO.DeviceDtos;
using TestProject.Models;
using TestProject.Query;
using TestProject.Services.DeviceTypeServices;

namespace TestProject.Services.DeviceServices
{
    public class DeviceService : TestProjectAppServiceBase, IDeviceService
    {
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;
        private readonly IRepository<DevicePropertyValue> _valueRepository;

        private readonly IDeviceTypeService _typeService;

        public DeviceService(IRepository<Device> deviceRepository,
            IRepository<DeviceTypeProperty> propertyRepository, IRepository<DevicePropertyValue> valueRepository,
            IDeviceTypeService typeService)
        {
            _deviceRepository = deviceRepository;
            _propertyRepository = propertyRepository;
            _valueRepository = valueRepository;
            _typeService = typeService;
        }


        // ------------ GET DEVICE/DEVICES ---------------//

        public List<DeviceDto> GetDevices()
        {
            var devices = _deviceRepository.GetAll().Include(x => x.DeviceType).ToList();

            var result = ObjectMapper.Map<List<DeviceDto>>(devices);

            return result;
        }



        // ---------------------------- CREATE OR UPDATE DEVICE ----------------------------//
        // --------------------------------- STEP 2 (last) ------------------------------//

        public UpdateDeviceDto GetDeviceTypesWithPropValues(int deviceId, int deviceTypeId)
        {
            var device = _deviceRepository.GetAll().Include(x => x.DeviceType).Include(x => x.DevicePropertyValues)
                .ThenInclude(x => x.DeviceTypeProperty).First(x => x.Id == deviceId);

            UpdateDeviceDto updatedDevice = new UpdateDeviceDto
            {
                DeviceId = deviceId,
                DeviceName = device.Name,
                Description = device.Description,
                TypeId = deviceTypeId,
                DeviceTypes = new List<UpdateDeviceTypesPropValuesDto>()
            };

            var typesForView = new List<UpdateDeviceTypesPropValuesDto>();

            var newTypes = _typeService.GetDeviceTypeWithParents(deviceTypeId).ToList();

            foreach (var newType in newTypes)
            {
                var typeForView = new UpdateDeviceTypesPropValuesDto
                {
                    DeviceTypeId = newType.Id,
                    DeviceTypeName = newType.Name,
                    PropValues = new List<UpdateDevicePropValueDto>()
                };

                foreach (var property in newType.DeviceTypeProperties)
                {
                    var valueForView =
                        device.DevicePropertyValues.FirstOrDefault(x => x.DeviceTypePropertyId == property.Id);
                    
                    var propValueForView = new UpdateDevicePropValueDto
                    {
                        PropName = property.Name,
                        Required = property.IsRequired,
                        Type = property.Type
                    };

                    if (valueForView == null)
                    {
                        propValueForView.Value = null;
                    }

                    typeForView.PropValues.Add(propValueForView);
                }

                typesForView.Add(typeForView);
            }

            var listOfTypes = typesForView.OrderBy(x => x.DeviceTypeId).ToList();

            updatedDevice.DeviceTypes = listOfTypes;

            return updatedDevice;
        }





        // ---------------------------- CREATE OR UPDATE DEVICE -----------------------------//
        // --------------------------------- STEP 3 (last)---------------------------------//
        public void CreateOrUpdateDevice(CreateDeviceDto device)
        {
            if (device.Id == 0)
            {
                var newDevice = new Device
                {
                    Name = device.DeviceName,
                    Description = device.Description,
                    DeviceTypeId = device.DeviceTypeId
                };

                foreach (var deviceType in device.DeviceTypes)
                {
                    var propValues = deviceType.PropValues;

                    foreach (var propValue in propValues)
                    {
                        if (propValue.Value != null)
                        {
                            _valueRepository.Insert(new DevicePropertyValue
                            {
                                Value = propValue.Value,
                                DeviceTypePropertyId = _propertyRepository.FirstOrDefault(x =>
                                    x.DeviceTypeId == deviceType.Id && x.Name == propValue.PropName).Id,
                                DeviceId = newDevice.Id
                            });
                        }
                    }
                }
                
                _deviceRepository.Insert(newDevice);

                return;
            }

            var targetDevice = _deviceRepository.GetAll().Include(x => x.DevicePropertyValues)
                .First(x => x.Id == device.Id);

            targetDevice.Name = device.DeviceName;
            targetDevice.Description = device.Description;

            if (targetDevice.DeviceTypeId != device.DeviceTypeId)
            {
                var oldTypes = _typeService.GetDeviceTypeWithParents(targetDevice.DeviceTypeId).ToList();

                var newTypes = _typeService.GetDeviceTypeWithParents(device.DeviceTypeId).ToList();

                foreach (var oldType in oldTypes)
                {
                    var type = newTypes.FirstOrDefault(x => x.Id == oldType.Id);

                    if (type == null)
                    {
                        foreach (var prop in oldType.DeviceTypeProperties)
                        {
                            var valueToDelete =
                                targetDevice.DevicePropertyValues.FirstOrDefault(x =>
                                    x.DeviceTypePropertyId == prop.Id);

                            if (valueToDelete == null) continue;

                            _valueRepository.Delete(valueToDelete);
                        }
                    }
                }
            }

            foreach (var deviceType in device.DeviceTypes)
            {
                foreach (var propValue in deviceType.PropValues)
                {
                    var property = _propertyRepository.GetAll().Include(x => x.DeviceType).First(x =>
                        x.DeviceTypeId == deviceType.Id && x.Name == propValue.PropName);

                    var values = _valueRepository.GetAll().Include(x => x.Device)
                        .Include(x => x.DeviceTypeProperty);

                    var value = values.FirstOrDefault(x =>
                        x.DeviceId == targetDevice.Id && x.DeviceTypePropertyId == property.Id);

                    if (propValue.Value != null)
                    {
                        if (value == null)
                        {
                            var newValue = new DevicePropertyValue
                            {
                                DeviceId = targetDevice.Id,
                                DeviceTypePropertyId = property.Id,
                                Value = propValue.Value
                            };

                            targetDevice.DevicePropertyValues.Add(newValue);

                            continue;
                        }

                        value.Value = propValue.Value;
                    }
                }
            }

            targetDevice.DeviceTypeId = device.DeviceTypeId;
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