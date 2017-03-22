using System;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace IoTMadridAccessControl.Repositories
{
    public class AccessControlRepository : IDisposable
    {
        private SqlConnection _sqlConnection;
        private RetryPolicy retryPolicy;

        public void testEHandler(object sender, RetryingEventArgs e)
        {
            //TODO log somewhere? mail?
            //Logger.war
        }

        public AccessControlRepository(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            // Define your retry strategy: retry 3 times, 1 second apart.
            var retryStrategy = new FixedInterval(3, TimeSpan.FromSeconds(1));

            // Define your retry policy using the retry strategy and the Azure storage
            // transient fault detection strategy.
            retryPolicy = new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying += testEHandler;           
        }

        public bool HasAccess(string accessDeviceId, int accessDeviceType)
        {
            var command = new SqlCommand(string.Format("select Id from AccessControlList where AccessDevice = '{0}' and AccessDeviceType = {1}", accessDeviceId, accessDeviceType), _sqlConnection);
            object result = null;
            try
            {
                _sqlConnection.OpenWithRetry(retryPolicy);
                result = command.ExecuteScalarWithRetry(retryPolicy);
            }
            catch(Exception e)
            {
                //either the number of retries specified in the policy is exceeded or there is another exception? 
                throw e;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return result != null;
        }

        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }
}