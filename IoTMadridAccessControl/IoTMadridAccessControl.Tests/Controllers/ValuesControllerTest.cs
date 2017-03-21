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
        public void GetByAccessDevice()
        {
            // Arrange
            AccessController controller = new AccessController();

            // Act
            var result = controller.Get("test", AccessDeviceType.LicensePlate);

            // Assert
            Assert.AreEqual(true, result);
        }

    }
}
