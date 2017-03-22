using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IoTMadridAccessControl.Repositories;

namespace IoTMadridAccessControl.Controllers
{
    public class AccessController : ApiController
    {
        private readonly AccessControlRepository AccessControlRepository;

        public AccessController()
        {
            AccessControlRepository = new AccessControlRepository("Server=tcp:iotmadrid.database.windows.net,1433;Initial Catalog=IOTMadrid;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;");
        }

        public bool Get(string accessDeviceId, AccessDeviceType accessDeviceType, int locationId)
        {
            return AccessControlRepository.HasAccess(accessDeviceId, (int)accessDeviceType, locationId);
        }
    }
}
