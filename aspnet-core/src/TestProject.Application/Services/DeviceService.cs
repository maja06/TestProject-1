using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Abp.Domain.Repositories;
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

        public DeviceService(IRepository<Device> deviceRepository, IRepository<DeviceType> deviceTypeRepository, IRepository<DeviceTypeProperty> propertyRepository, IRepository<DevicePropertyValue> valueRepository)
        {
            _deviceRepository = deviceRepository;
            _deviceTypeRepository = deviceTypeRepository;
            _propertyRepository = propertyRepository;
            _valueRepository = valueRepository;
        }

        //public Device CreateDeviceByType(DeviceType deviceType)
        //{
        //    Device device = new Device();

        //    device.DeviceType = deviceType;
        //    device.DeviceTypeProperties = RecursionForProperties(deviceType);

        //    return device;
        //}



        //public void CreateDevice(Device device)
        //{
        //    var newDevice = CreateDeviceByType(device.DeviceType);
        //}


        public void CreateType(DeviceType deviceType)
        {
            DeviceType device = new DeviceType();

            device.Name = deviceType.Name;

        }

        public void CreateDevice(Device device)
        {
            Device newDevice = new Device();

            newDevice.DeviceType = device.DeviceType;
            
            var values = device.DevicePropertyValues;

            foreach (var value in values)
            {
                value.DeviceTypeProperty = RecursionForProperties(newDevice.DeviceType).FirstOrDefault(x => x.Id == value.DeviceTypePropertyId);
            }

        }





        public IEnumerable<IGrouping<int, DeviceTypeProperty>> PropertiesByType(List<DeviceTypeProperty> listOfProperties)
        {
            var propertiesByType = listOfProperties.GroupBy(x => x.DeviceTypeId);

            return propertiesByType;
        }




        public List<DeviceTypeProperty> RecursionForProperties(DeviceType deviceType)
        {
            List<DeviceTypeProperty> properties = new List<DeviceTypeProperty>();

            var type = _deviceTypeRepository.GetAll().Include(x => x.ParentDeviceType).FirstOrDefault(x => x.Id == deviceType.Id);

            var currentType = type;

            while (currentType != null)
            {
                foreach (var property in currentType.DeviceTypeProperties)
                {
                    properties.Add(property);
                }

                currentType = currentType.ParentDeviceType;
            }

            return properties;
        }


    }

}
