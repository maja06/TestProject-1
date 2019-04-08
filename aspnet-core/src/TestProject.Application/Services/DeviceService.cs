using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using TestProject.DTO;
using TestProject.Models;

namespace TestProject.Services
{
    public class DeviceService : TestProjectAppServiceBase, IDeviceService
    {
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<DeviceType> _deviceTypeRepository;
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;
        private readonly IRepository<DevicePropertyValue> _valueRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceService"/> class.
        /// </summary>
        /// <param name="deviceRepository">The device repository.</param>
        /// <param name="deviceTypeRepository">The device type repository.</param>
        /// <param name="propertyRepository">The property repository.</param>
        /// <param name="valueRepository">The value repository.</param>
        public DeviceService(IRepository<Device> deviceRepository, IRepository<DeviceType> deviceTypeRepository, IRepository<DeviceTypeProperty> propertyRepository, IRepository<DevicePropertyValue> valueRepository)
        {
            _deviceRepository = deviceRepository;
            _deviceTypeRepository = deviceTypeRepository;
            _propertyRepository = propertyRepository;
            _valueRepository = valueRepository;
        }



        //------------- GET TYPES/TYPE ---------------//

        public IEnumerable<DeviceTypePropertiesDto> GetdDeviceTypePropertiesNestedDtos(int? deviceTypeId)
        {
            var type = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
                .First(x => x.Id == deviceTypeId);

            var result = new List<DeviceTypePropertiesDto>();

            var currentType = new DeviceTypePropertiesDto()
            {
                Id = type.Id,
                Name = type.Name,
                Description = type.Description,
                ParentId = type.ParentDeviceTypeId,
                Properties = ObjectMapper.Map<List<DeviceTypePropertyDto>>(type.DeviceTypeProperties)

            };

            if (type.ParentDeviceTypeId == null)
            {
                result.Add(currentType);
                return result;
            }

            result.Add(currentType);

            return result.Concat(GetdDeviceTypePropertiesNestedDtos(type.ParentDeviceTypeId)).OrderBy(x => x.Id);

        }

        public List<DeviceTypeNestedDto> GetDeviceTypeNestedDtos(int? parentId)
        {
            var baseDeviceTypes = _deviceTypeRepository.GetAll()
                .Where(x => x.ParentDeviceTypeId == parentId).ToList();

            var result = new List<DeviceTypeNestedDto>();

            foreach (var deviceType in baseDeviceTypes)
            {
                var currentType = new DeviceTypeNestedDto
                {
                    Id = deviceType.Id,
                    Name = deviceType.Name,
                    Description = deviceType.Description,
                    ParentId = deviceType.ParentDeviceTypeId,
                    Children = GetDeviceTypeNestedDtos(deviceType.Id)
                };

                result.Add(currentType);
            }

            return result;
        }

        public DeviceType GetDeviceTypeById(int id)
        {
            var deviceType = _deviceTypeRepository.Get(id);

            return deviceType;
        }



        //------------- GET DEVICES/DEVICE ---------------//

        public List<Device> GetAllDevices()
        {
            var devices = _deviceRepository.GetAll().Include(x => x.DeviceType).ThenInclude(x => x.DeviceTypeProperties).ToList();

            return devices;
        }
        
        public Device GetDeviceById(int id)
        {
            var device = _deviceRepository.GetAll().Include(x => x.DeviceType).ThenInclude(x => x.DeviceTypeProperties).FirstOrDefault(x => x.Id == id);

            return device;
        }



        //------------- CREATE TYPE ---------------//

        public IEnumerable<DeviceTypePropertiesDto> CreateOrUpdateDeviceType(DeviceTypeDto deviceTypeDto)
        {
            if (deviceTypeDto.Id == 0)
            {
                DeviceType newDeviceType = ObjectMapper.Map<DeviceType>(deviceTypeDto);

                var id = _deviceTypeRepository.InsertAndGetId(newDeviceType);

                var deviceTypes = GetdDeviceTypePropertiesNestedDtos(id);

                return deviceTypes;
            }

            var targetDeviceType = _deviceTypeRepository.Get(deviceTypeDto.Id);

            ObjectMapper.Map(deviceTypeDto, targetDeviceType);

            var updatedDeviceTypes = GetdDeviceTypePropertiesNestedDtos(targetDeviceType.Id);

            return updatedDeviceTypes;
        }


        //------------- CREATE DEVICE ---------------//

        public void UpdateDeviceTypeProperties(DeviceTypePropertiesDto deviceTypePoPropertiesDto)
        {
            var deviceType = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
                .First(x => x.Id == deviceTypePoPropertiesDto.Id);

            foreach (var property in deviceTypePoPropertiesDto.Properties)
            {
                deviceType.DeviceTypeProperties.Add(ObjectMapper.Map<DeviceTypeProperty>(property));
            }
            
        }


        

        //------------- RECURSIONS FOR PROPERTIES ---------------//

        //public IEnumerable<DeviceTypeProperty> RecursionForType(int? id)
        //{
        //    var deviceType = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
        //        .Include(x => x.ParentDeviceType).FirstOrDefault(x => x.ParentDeviceTypeId == id);

        //    if(deviceType == null) throw new UserFriendlyException("No Device Type at given Id");

        //    List<DeviceTypeProperty> properties = deviceType.DeviceTypeProperties;
            
        //    if (deviceType.ParentDeviceTypeId == null)
        //    {
        //        return properties;
        //    }
            
        //    return properties.Concat(RecursionForType(deviceType.Id));
        //}

        

        //------------- RECURSIONS FOR TYPE ---------------//

        //public IEnumerable<DeviceType> ListOfDeviceTypes(int? id)
        //{
        //   var deviceType = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
        //        .Include(x => x.ParentDeviceType).FirstOrDefault(x => x.ParentDeviceTypeId == id);

        //   var result = new List<DeviceType>();

        //   if (deviceType.ParentDeviceTypeId == null)
        //   {
        //       result.Add(deviceType);
        //       return result;
        //   }

        //   result.Add(deviceType);
        //   return result.Concat(ListOfDeviceTypes(deviceType.ParentDeviceTypeId));
        //}

    }

}
