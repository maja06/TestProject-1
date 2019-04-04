using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
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

        public List<DeviceType> GetAllDeviceTypes()
        {
            var deviceTypes = _deviceTypeRepository.GetAll().Include(x => x.ParentDeviceType).ToList();

            return deviceTypes;
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

           var deviceTypes = new List<DeviceType>();
           deviceTypes.Add(deviceType.ParentDeviceType);

           if (deviceType.ParentDeviceTypeId == null)
           {
               deviceTypes.Add(deviceType);
               return deviceTypes;
           }

            return deviceTypes.Concat(ListOfDeviceTypes(deviceType.ParentDeviceTypeId));
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
