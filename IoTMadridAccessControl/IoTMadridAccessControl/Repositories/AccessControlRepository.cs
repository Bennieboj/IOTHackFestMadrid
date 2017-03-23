using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public bool HasAccess(string accessDeviceId, int accessDeviceType, int locationId)
        {
            var command = new SqlCommand(string.Format(@"select Id from AccessControlList where AccessDevice = '{0}' and AccessDeviceType = {1} and LocationId = {2}", accessDeviceId, accessDeviceType, locationId), _sqlConnection);
            object result = null;
            try
            {
                _sqlConnection.OpenWithRetry(retryPolicy);
                result = command.ExecuteScalarWithRetry(retryPolicy);
            }
            catch (Exception e)
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

        public List<TimeSlot> GetServiceProfile(string accessDeviceId, int accessDeviceType, int locationId)
        {
            var query = string.Format(@"
select sp.Name, t.DayOfWeek, t.StartHour, t.StartMinutes, t.EndHour, t.EndMinutes
from AccessControlList acl
join ServiceProfile sp on sp.Id = acl.ServiceProfileId
join Timeslot t on t.ServiceProfileId = sp.Id
where AccessDevice = '{0}' and AccessDeviceType = {1} and LocationId = {2}", accessDeviceId, accessDeviceType, locationId);
            List<TimeSlot> convertedList;
            try
            {
                _sqlConnection.OpenWithRetry(retryPolicy);
                var table = new DataTable();
                using (var da = new SqlDataAdapter(query, _sqlConnection))
                {
                    da.Fill(table);

                   convertedList = (from rw in table.AsEnumerable()
                   select new TimeSlot()
                   {
                       ServiceProfileName = Convert.ToString(rw["Name"]),
                       DayOfWeek = Convert.ToInt32(rw["DayOfWeek"]),
                       StartHour = Convert.ToInt32(rw["StartHour"]),
                       StartMinutes = Convert.ToInt32(rw["StartMinutes"]),
                       EndHour = Convert.ToInt32(rw["EndHour"]),
                       EndMinutes = Convert.ToInt32(rw["EndMinutes"])
                   }).ToList();
                }
            }
            catch (Exception e)
            {
                //either the number of retries specified in the policy is exceeded or there is another exception? 
                throw e;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return convertedList;
        }

        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }
}