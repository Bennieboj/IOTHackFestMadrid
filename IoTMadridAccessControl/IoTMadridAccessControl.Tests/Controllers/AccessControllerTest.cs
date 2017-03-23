using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IoTMadridAccessControl;
using IoTMadridAccessControl.Controllers;

namespace IoTMadridAccessControl.Tests.Controllers
{
    [TestClass]
    public class AccessControllerTest
    {


        [TestMethod]
        public void CanGetHasAccess()
        {
            // Arrange
            AccessController controller = new AccessController();

            // Act
            var result = controller.HasAccess("H647KPE", AccessDeviceType.LicensePlate, 1);

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CanGetServiceProfile()
        {
            // Arrange
            AccessController controller = new AccessController();

            // Act
            var result = controller.GetServiceProfile("H647KPE", AccessDeviceType.LicensePlate, 1);

            // Assert
            Assert.AreEqual("24x7", result.First().ServiceProfileName);
        }

    }
}
