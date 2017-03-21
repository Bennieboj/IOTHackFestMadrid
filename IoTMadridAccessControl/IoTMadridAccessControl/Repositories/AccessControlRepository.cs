using System;
using System.Data.SqlClient;

namespace IoTMadridAccessControl.Repositories
{
    public class AccessControlRepository : IDisposable
    {
        private SqlConnection _sqlConnection;

        public AccessControlRepository(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
        }

        public bool HasAccess(string accessDeviceId, int accessDeviceType)
        {
            var command = new SqlCommand(string.Format("select Id from AccessControlList where AccessDevice = '{0}' and AccessDeviceType = {1}", accessDeviceId, accessDeviceType), _sqlConnection);
            _sqlConnection.Open();
            var result = command.ExecuteScalar();
            _sqlConnection.Close();
            return result != null;
        }

        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }
}