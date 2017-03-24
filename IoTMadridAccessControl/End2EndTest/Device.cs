    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace IoTMadridAccessControl.End2EndTest
{
    class IOTDevice
    {
        static RegistryManager registryManager;
        static string connectionString = @"HostName=OctoParkHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=ttonSz5GzvlaAZ5IY9+ZhwHmW9r6ZA77Aqd7uC6lcrs=";

        public static async Task<string> AddorGetDevice(string deviceId)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            return device.Authentication.SymmetricKey.PrimaryKey;
        }
    }
    }