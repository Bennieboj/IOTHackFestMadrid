using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace IoTMadridAccessControl.End2EndTest
{
    class IOTDeviceClient
    {
        static DeviceClient deviceClient;
        static string iotHubUri = @"OctoParkHub.azure-devices.net";

        public static async Task SendDeviceToCloudMessage(string deviceId, string deviceKey)
        {
            // AUTH / REQUEST is the topic of the message. It is included in both the body of the message and as a property of the message.
            //            Expected response:
            //{ messageType: 'AUTH/SUCCESS', correlation_id: '123456789012' }

            var messageString = "{type: 'AUTH/REQUEST',device: '" + deviceId +
                "',locationId: 1,accessDeviceType: 1,accessDeviceValue: 'LK53ABY',correlation_id: '123456789012',timestamp: '" +
                DateTime.UtcNow + "',direction: 'in'})"; 

            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            message.Properties.Add("topic", "AUTH/REQUEST");

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);
                         
            try
            {
                await deviceClient.SendEventAsync(message);
            }
            catch (Exception e)
            {
                var m = e.Message;
            }

            //while (true)
            //{
            //    Message receivedMessage = await deviceClient.ReceiveAsync();
            //    if (receivedMessage == null) continue;

            //    var t = Encoding.ASCII.GetString(receivedMessage.GetBytes());

            //    await deviceClient.CompleteAsync(receivedMessage);
            //}

        }
    }
}
