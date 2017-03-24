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
        private ReliableSqlConnection _sqlConnection;
        private RetryPolicy retryPolicy;

        public void testEHandler(object sender, RetryingEventArgs e)
        {
            //TODO log somewhere? mail?
            //Logger.war
        }

        public AccessControlRepository(string connectionString)
        {
            // Define your retry strategy: retry 3 times, 1 second apart.
            var retryStrategy = new FixedInterval(3, TimeSpan.FromSeconds(1));

            // Define your retry policy using the retry strategy and the Azure storage
            // transient fault detection strategy.
            retryPolicy = new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying += testEHandler;
            _sqlConnection = new ReliableSqlConnection(connectionString, retryPolicy, retryPolicy);
        }

        public bool HasAccess(string accessDeviceId, int accessDeviceType, int locationId)
        {
            var command = _sqlConnection.CreateCommand();
            command.CommandText = string.Format(@"select Id from AccessControlList where AccessDevice = '{0}' and AccessDeviceType = {1} and LocationId = {2}", accessDeviceId, accessDeviceType, locationId);
            var result = ExecuteQuery(command);

            if (result == null)
            {
                return false;
            }


            //check pool
            List<Pool> poolList = new List<Pool>();
            try
            {
                _sqlConnection.Open();
                var poolCommand = _sqlConnection.CreateCommand();
                poolCommand.CommandText = string.Format(@"
select distinct p.Id, p.MaxAllowed, p.Occupied, p.Hard
from AccessControlList acl
join Pool p on p.Id = acl.PoolId
where acl.Id = {0}", result);

                using (var reader = poolCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        poolList.Add(new Pool
                        {
                            Id = reader.GetInt32(0),
                            MaxAllowed = reader.GetInt32(1),
                            Occupied = reader.GetInt32(2),
                            Hard = reader.GetBoolean(3)
                        });
                    }
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

            if (poolList.Any())
            {
                var pool = poolList.Single();
                if (pool.Hard && pool.MaxAllowed <= pool.Occupied)
                {
                    return false;
                }
                var updatePoolCommand = _sqlConnection.CreateCommand();
                updatePoolCommand.CommandText = string.Format(@"Update Pool set Occupied= {0} where Id = {1}", ++pool.Occupied, pool.Id);
                try
                {
                    _sqlConnection.Open();
                    updatePoolCommand.ExecuteNonQuery();
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
            }

            return true;
        }

        public bool CanExit(string accessDeviceId, int accessDeviceType, int locationId)
        {
            var command = _sqlConnection.CreateCommand();
            command.CommandText = string.Format(@"select Id from AccessControlList where AccessDevice = '{0}' and AccessDeviceType = {1} and LocationId = {2}", accessDeviceId, accessDeviceType, locationId);
            var result = ExecuteQuery(command);

            if (result == null)
            {
                return false;
            }


            //check pool
            List<Pool> poolList = new List<Pool>();
            try
            {
                _sqlConnection.Open();
                var poolCommand = _sqlConnection.CreateCommand();
                poolCommand.CommandText = string.Format(@"
select distinct p.Id, p.MaxAllowed, p.Occupied, p.Hard
from AccessControlList acl
join Pool p on p.Id = acl.PoolId
where acl.Id = {0}", result);

                using (var reader = poolCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        poolList.Add(new Pool
                        {
                            Id = reader.GetInt32(0),
                            MaxAllowed = reader.GetInt32(1),
                            Occupied = reader.GetInt32(2),
                            Hard = reader.GetBoolean(3)
                        });
                    }
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

            if (poolList.Any())
            {
                var pool = poolList.Single();
                var updatePoolCommand = _sqlConnection.CreateCommand();
                updatePoolCommand.CommandText = string.Format(@"Update Pool set Occupied= {0} where Id = {1}", Math.Max(--pool.Occupied, 0), pool.Id);
                try
                {
                    _sqlConnection.Open();
                    updatePoolCommand.ExecuteNonQuery();
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
            }

            return true;
        }

        private object ExecuteQuery(SqlCommand command)
        {
            object result = null;
            try
            {
                _sqlConnection.Open();
                result = command.ExecuteScalar();
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
            return result;
        }

        public List<TimeSlot> GetServiceProfile(string accessDeviceId, int accessDeviceType, int locationId)
        {
            var query = string.Format(@"
select sp.Name, t.DayOfWeek, t.StartHour, t.StartMinutes, t.EndHour, t.EndMinutes
from AccessControlList acl
join ServiceProfile sp on sp.Id = acl.ServiceProfileId
join Timeslot t on t.ServiceProfileId = sp.Id
where AccessDevice = '{0}' and AccessDeviceType = {1} and LocationId = {2}", accessDeviceId, accessDeviceType, locationId);
            
            List<TimeSlot> convertedList = new List<TimeSlot>();
            try
            {
                _sqlConnection.Open();
                var command = _sqlConnection.CreateCommand();
                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        convertedList.Add(new TimeSlot
                        {
                            ServiceProfileName = reader.GetString(0),
                            DayOfWeek = reader.GetInt32(1),
                            StartHour = reader.GetInt32(2),
                            StartMinutes = reader.GetInt32(3),
                            EndHour = reader.GetInt32(4),
                            EndMinutes = reader.GetInt32(5)
                        });
                    }
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