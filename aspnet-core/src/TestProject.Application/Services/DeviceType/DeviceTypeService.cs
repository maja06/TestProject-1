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
    public class DeviceTypeService : TestProjectAppServiceBase, IDeviceTypeService
    {
        private readonly IRepository<DeviceType> _deviceTypeRepository;
        private readonly IRepository<DeviceTypeProperty> _propertyRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceService"/> class.
        /// </summary>
        /// <param name="deviceRepository">The device repository.</param>
        /// <param name="deviceTypeRepository">The device type repository.</param>
        /// <param name="propertyRepository">The property repository.</param>
        /// <param name="valueRepository">The value repository.</param>
        public DeviceTypeService(IRepository<DeviceType> deviceTypeRepository, IRepository<DeviceTypeProperty> propertyRepository)
        {
            _deviceTypeRepository = deviceTypeRepository;
            _propertyRepository = propertyRepository;
        }



        //------------- GET TYPES/TYPE ---------------//

        public IEnumerable<DeviceTypePropertiesDto> GetdDeviceTypePropertiesDtos(int? deviceTypeId)
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

            return result.Concat(GetdDeviceTypePropertiesDtos(type.ParentDeviceTypeId)).OrderBy(x => x.Id);

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


        //------------- CREATE TYPE ---------------//

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


        //------------- CREATE PROPERTIES FOR TYPE ---------------//

        public void UpdateDeviceTypeProperties(DeviceTypePropertyUpdateDto deviceTypePropertyUpdateDto)
        {
            var deviceType = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
                .First(x => x.Id == deviceTypePropertyUpdateDto.Id);

            foreach (var property in deviceTypePropertyUpdateDto.Properties)
            {
                _propertyRepository.Insert(new DeviceTypeProperty
                {
                    Name = property.NameProperty,
                    IsRequired = property.Required,
                    Type = property.Type,
                    DeviceTypeId = deviceType.Id
                });
            }
        }
    }

}
