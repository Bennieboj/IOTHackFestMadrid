using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoTMadridAccessControl.End2EndTest
{
    [TestClass]
    public class AccessIOTHubTest
    {

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }



        [TestMethod]
        public void CanAccessIOTHub()
        {
            //TODO: parameterize Accesdevice id en type 
           var deviceId = "TestDevice";
           var deviceKey = IOTDevice.AddorGetDevice(deviceId).Result;
           IOTDeviceClient.SendDeviceToCloudMessage(deviceId, deviceKey).Wait();
        
        }
    }
}
