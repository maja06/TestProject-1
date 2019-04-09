using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using TestProject.DTO.DeviceTypeDtos;
using TestProject.Models;
using TestProject.Services.DeviceServices;

namespace TestProject.Services.DeviceTypeServices
{
    public class DeviceTypeService : TestProjectAppServiceBase, IDeviceTypeService
    {
        private readonly IRepository<DeviceType> _deviceTypeRepository;
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<DevicePropertyValue> _valueRepository;


        /// <summary>
        ///     Initializes a new instance of the <see cref="DeviceService" /> class.
        /// </summary>
        /// <param name="deviceRepository">The device repository.</param>
        /// <param name="deviceTypeRepository">The device type repository.</param>
        /// <param name="propertyRepository">The property repository.</param>
        /// <param name="valueRepository">The value repository.</param>
        public DeviceTypeService(IRepository<DeviceType> deviceTypeRepository,
            IRepository<DeviceTypeProperty> propertyRepository, IRepository<Device> deviceRepository, IRepository<DevicePropertyValue> valueRepository)
        {
            _deviceTypeRepository = deviceTypeRepository;
            _propertyRepository = propertyRepository;
            _deviceRepository = deviceRepository;
            _valueRepository = valueRepository;
        }



        //------------------------GET FLAT LIST OF TYPES WITH PROPERTIES -------------------------//

        public IEnumerable<DeviceTypePropertiesDto> GetdDeviceTypePropertiesDtos(int? deviceTypeId)
        {
            var type = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
                .First(x => x.Id == deviceTypeId);

            var result = new List<DeviceTypePropertiesDto>();

            var currentType = new DeviceTypePropertiesDto
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

            return result.Concat(GetdDeviceTypePropertiesDtos(type.ParentDeviceTypeId)).OrderBy(x => x.Id);
        }



        // ------------------------------ GET NESTED LIST OF TYPES -------------------------------//

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



        //-------------------------- CREATE NEW TYPE -------------------------------//

        public IEnumerable<DeviceTypePropertiesDto> CreateOrUpdateDeviceType(DeviceTypeDto deviceTypeDto)
        {
            if (deviceTypeDto.Id == 0)
            {
                DeviceType newDeviceType = ObjectMapper.Map<DeviceType>(deviceTypeDto);

                var id = _deviceTypeRepository.InsertAndGetId(newDeviceType);

                var deviceTypes = GetdDeviceTypePropertiesDtos(id);

                return deviceTypes;
            }

            var targetDeviceType = _deviceTypeRepository.Get(deviceTypeDto.Id);

            ObjectMapper.Map(deviceTypeDto, targetDeviceType);

            var updatedDeviceTypes = GetdDeviceTypePropertiesDtos(targetDeviceType.Id);

            return updatedDeviceTypes;
        }



        //---------------------- CREATE PROPERTIES FOR TYPE -----------------------//

        public void UpdateDeviceTypeProperties(DeviceTypePropertyUpdateDto deviceTypePropertyUpdateDto)
        {
            var deviceType = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
                .First(x => x.Id == deviceTypePropertyUpdateDto.Id);

            foreach (var property in deviceTypePropertyUpdateDto.Properties)
                _propertyRepository.Insert(new DeviceTypeProperty
                {
                    Name = property.NameProperty,
                    IsRequired = property.Required,
                    Type = property.Type,
                    DeviceTypeId = deviceType.Id
                });
        }

        


        // --------------------------- DELETE TYPE ------------------------------//

        public void Delete(int id)
        {
            var types = GetTypesToDelete(id).ToList();

            var orderedTypes = types.OrderByDescending(x => x.Id);

            foreach (var type in orderedTypes)
            {
                var devices = type.Devices;

                foreach (var device in devices)
                {
                    var values = device.DevicePropertyValues;

                    foreach (var value in values)
                    {
                        _valueRepository.Delete(value);
                    }

                    _deviceRepository.Delete(device);
                }

                _deviceTypeRepository.Delete(type);
            }
        }



        // ------------------- GET LIST OF SUBTYPES TO DELETE ---------------------//

        public IEnumerable<DeviceType> GetTypesToDelete(int parentId)
        {
            var type = _deviceTypeRepository.GetAll().Include(x => x.Devices).ThenInclude(x => x.DevicePropertyValues)
                .Include(x => x.DeviceTypeProperties)
                .First(x => x.Id == parentId);

            var children = _deviceTypeRepository.GetAll().Include(x => x.Devices).ThenInclude(x => x.DevicePropertyValues)
                .Include(x => x.DeviceTypeProperties)
                .Where(x => x.ParentDeviceTypeId == parentId).ToList();

            var list = new List<DeviceType>();

            if (!children.Any())
            {
                list.Add(type);
                return list;
            }

            foreach (var child in children)
            {
                list.AddRange(GetTypesToDelete(child.Id));
            }

            list.Add(type);
            return list;

        }
    }
}