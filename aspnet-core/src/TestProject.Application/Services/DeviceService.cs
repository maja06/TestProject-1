using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Abp.Domain.Repositories;
using Abp.UI;
using Abp.UI.Inputs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
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

        public List<DeviceTypeForListDto> GetAllDeviceTypes()
        {
            var deviceTypes = _deviceTypeRepository.GetAll().Include(x => x.ParentDeviceType).ToList();

            var result = ObjectMapper.Map<List<DeviceTypeForListDto>>(deviceTypes);

            return result;
        }

        public List<DeviceTypeDto> ListOfDeviceTypePorperties(int? id)
        {
            var deviceTypes = ListOfDeviceTypes(id);

            var result = new List<DeviceTypeDto>();

            foreach (var deviceType in deviceTypes)
            {
                var deviceTypeDto = new DeviceTypeDto();

                deviceTypeDto.Id = deviceType.Id;
                deviceTypeDto.name = deviceType.Name;
                deviceTypeDto.description = deviceType.Description;
                deviceTypeDto.parentid = deviceType.ParentDeviceTypeId;
                deviceTypeDto.properties =
                    ObjectMapper.Map<List<DeviceTypePropertyDto>>(deviceType.DeviceTypeProperties);

                result.Add(deviceTypeDto);
            }

            return result;
        }

        public List<DeviceTypeNestedDto> GetNestedDeviceTypeForListDto(int? parentId)
        {
            var baseDeviceTypes = _deviceTypeRepository.GetAll()
                .Where(x => x.ParentDeviceTypeId == parentId).ToList();

            var result = new List<DeviceTypeNestedDto>();

            foreach (var deviceType in baseDeviceTypes)
            {
                var currentType = new DeviceTypeNestedDto
                {
                    Id = deviceType.Id,
                    name = deviceType.Name,
                    description = deviceType.Description,
                    children = GetNestedDeviceTypeForListDto(deviceType.Id)
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

        public void CreateDeviceType(DeviceType deviceType)
        {
            

        }


        //------------- CREATE DEVICE ---------------//

        public void CreateDevice(Device device)
        {
            Device newDevice = new Device();

            


        }



        //------------- RECURSIONS FOR PROPERTIES ---------------//

        





        //------------- RECURSIONS FOR PROPERTIES ---------------//

        public IEnumerable<DeviceTypeProperty> RecursionForType(int? id)
        {
            var deviceType = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
                .Include(x => x.ParentDeviceType).FirstOrDefault(x => x.Id == id);

            if(deviceType == null) throw new UserFriendlyException("No Device Type at given Id");

            List<DeviceTypeProperty> properties = deviceType.DeviceTypeProperties;
            
            if (deviceType.ParentDeviceTypeId == null)
            {
                return properties;
            }
            
            return properties.Concat(RecursionForType(deviceType.ParentDeviceTypeId));
        }
        
        //------------- RECURSIONS FOR TYPE ---------------//

        public IEnumerable<DeviceType> ListOfDeviceTypes(int? id)
        {

            var deviceType = _deviceTypeRepository.GetAll().Include(x => x.DeviceTypeProperties)
                .Include(x => x.ParentDeviceType).FirstOrDefault(x => x.Id == id);

            int? parentId = deviceType.ParentDeviceTypeId;

           var deviceTypes = new List<DeviceType>();
           deviceTypes.Add(deviceType);

           if (parentId == null)
           {
               return deviceTypes;
           }

            return deviceTypes.Concat(ListOfDeviceTypes(parentId));
        }

        //public List<DeviceTypeProperty> RecursionForProperties(DeviceType deviceType)
        //{
        //    List<DeviceTypeProperty> properties = new List<DeviceTypeProperty>();

        //    var type = _deviceTypeRepository.GetAll().Include(x => x.ParentDeviceType).FirstOrDefault(x => x.Id == deviceType.Id);

        //    var currentType = type;

        //    while (currentType != null)
        //    {
        //        foreach (var property in currentType.DeviceTypeProperties)
        //        {
        //            properties.Add(property);
        //        }

        //        currentType = currentType.ParentDeviceType;
        //    }

        //    return properties;
        //}
    }

}
