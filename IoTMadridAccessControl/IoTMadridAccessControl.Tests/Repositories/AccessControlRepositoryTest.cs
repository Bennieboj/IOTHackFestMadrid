using System.Data.SqlClient;
using IoTMadridAccessControl.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace IoTMadridAccessControl.Tests.Repositories
{
    [TestClass]
    public class AccessControlRepositoryTest
    {
        private static readonly string _masterConnectionString = "Server=.;Initial Catalog=master;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;";
        private static readonly string _testDbConnectionString = "Server=.;Initial Catalog=TESTIOTMadrid;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;";
        //private static readonly string _masterConnectionString = "Server=tcp:iotmadrid.database.windows.net,1433;Initial Catalog=master;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=100;";
        private static readonly string _testDbConnectionStringAzure = "Server=tcp:iotmadrid.database.windows.net,1433;Initial Catalog=IOTMadrid;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;";
        private static readonly SqlConnection _masterConnection = new SqlConnection(_masterConnectionString);
        private static readonly SqlConnection _testDbConnection = new SqlConnection(_testDbConnectionString);


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            //create db
            _masterConnection.Open();
            var command = new SqlCommand(@"
IF EXISTS(select * from sys.databases where name='TESTIOTMadrid')
DROP DATABASE [TESTIOTMadrid];
CREATE DATABASE [TESTIOTMadrid];
", _masterConnection);
            command.CommandTimeout = 100;
            command.ExecuteNonQuery();
            _masterConnection.Close();
        }
        [ClassCleanup]
        public static void ClassCleanup()
        {
            SqlConnection.ClearPool(_testDbConnection);
            _testDbConnection.Close();
            _masterConnection.Open();
            var command = new SqlCommand(@"
DROP DATABASE [TESTIOTMadrid];
", _masterConnection);
            command.ExecuteNonQuery();
            _masterConnection.Close();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            //clean db
            var _testDbConnection = new SqlConnection(_testDbConnectionString);
            _testDbConnection.Open();
            var command = new SqlCommand(@"
if OBJECT_ID('dbo.AccessControlList_Pool') is not null
drop table AccessControlList_Pool
if OBJECT_ID('dbo.Pool') is not null
drop table Pool
if OBJECT_ID('dbo.AccessControlList') is not null
drop table AccessControlList
if OBJECT_ID('dbo.TimeSlot') is not null
drop table TimeSlot
if OBJECT_ID('dbo.ServiceProfile') is not null
drop table ServiceProfile
", _testDbConnection);
            command.ExecuteNonQuery();
            
            var command2 = new SqlCommand(@"
CREATE TABLE [dbo].[ServiceProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

CREATE TABLE [dbo].[TimeSlot](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServiceProfileId] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL DEFAULT (1),
	[StartHour] [int] NOT NULL DEFAULT ((1)),
	[StartMinutes] [int] NOT NULL DEFAULT ((1)),
	[EndHour] [int] NOT NULL DEFAULT ((1)),
	[EndMinutes] [int] NOT NULL DEFAULT ((1)),
	FOREIGN KEY (ServiceProfileId) REFERENCES ServiceProfile(Id)
)

create table [dbo].[accesscontrollist](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccessDevice] [varchar](255) NOT NULL,
	[AccessDeviceType] [int] NOT NULL,
	[LocationId] [int] NOT NULL DEFAULT ((1)),
	[ServiceProfileId] [int] NOT NULL DEFAULT ((1)),
	[PoolId] [int] NULL,
	foreign key (serviceprofileid) references serviceprofile(Id),
constraint [adadtypelocationid] primary key nonclustered 
(
	[accessdevice] asc,
	[accessdevicetype] asc,
	[locationid] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
);

CREATE TABLE [dbo].[Pool](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MaxAllowed] [int] NOT NULL,
	[Occupied] [int] NOT NULL,
	[Hard] [bit] NOT NULL
);
", _testDbConnection);
            command2.ExecuteNonQuery();
            _testDbConnection.Close();
        }

        [TestMethod]
        [Ignore]
        public void tesst()
        {
            var sqlConnection = new SqlConnection(_testDbConnectionStringAzure);
            SqlBulkCopy bulk = new SqlBulkCopy(sqlConnection);
            bulk.DestinationTableName = "AccessControlList";
            
            sqlConnection.Open();
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AccessDevice", "AccessDevice"));
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AccessDeviceType", "AccessDeviceType"));
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("LocationId", "LocationId"));
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("ServiceProfileId", "ServiceProfileId"));

            for (int j = 0; j < 20; j++)
            {
                var table = new DataTable();
                table.Columns.AddRange(new[] { new DataColumn("AccessDevice", typeof(string)), new DataColumn("AccessDeviceType", typeof(int)), new DataColumn("LocationId", typeof(int)), new DataColumn("ServiceProfileId", typeof(int)) });

                for (int i = 0; i < 100000; i++)
                {
                    var row = table.NewRow();
                    row.ItemArray = new object[] { Guid.NewGuid().ToString().ToUpperInvariant(), 1, 1, 1 };
                    table.Rows.Add(row);
                }
                bulk.WriteToServer(table);
            }
        }

        [TestMethod]
        public void HasAccess_NotFoundEmpty_ReturnsFalse()
        {
            // Arrange
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1, 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void HasAccess_NotFoundWrongRecord_Type_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 2, 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void HasAccess_NotFoundWrongRecord_Name_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("othername", 1, 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void HasAccess_NotFoundWrongRecord_Location_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1, 2);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CanExit_NotFoundWrongRecord_Type_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.CanExit("test", 2, 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CanExit_NotFoundWrongRecord_Name_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.CanExit("othername", 1, 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CanExit_NotFoundWrongRecord_Location_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.CanExit("test", 1, 2);

            // Assert
            Assert.AreEqual(false, result);
        }


        [TestMethod]
        public void CanExit_WithPool_Occupied1()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            int poolId = InsertPool(1, 1, 1);
            InsertAccessControlListValue("test", 1, 1, serviceProfileId, poolId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.CanExit("test", 1, 1);

            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(0, GetPoolOccupied("test", 1, 1));
        }

        [TestMethod]
        public void CanExit_WithPool_Occupied0()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            int poolId = InsertPool(1, 1, 1);
            InsertAccessControlListValue("test", 1, 1, serviceProfileId, poolId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.CanExit("test", 1, 1);

            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(0, GetPoolOccupied("test", 1, 1));
        }

        [TestMethod]
        public void HasAccess_WithPoolUnderMaxAllowed_ReturnsTrue()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            int poolId = InsertPool(1, 0, 1);
            InsertAccessControlListValue("test", 1, 1, serviceProfileId, poolId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1, 1);

            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(1, GetPoolOccupied("test", 1, 1));
        }

        [TestMethod]
        public void HasAccess_WithPoolEqualToMaxAllowed_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            int poolId = InsertPool(1, 1, 1);
            InsertAccessControlListValue("test", 1, 1, serviceProfileId, poolId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1, 1);

            // Assert
            Assert.AreEqual(false, result);
            Assert.AreEqual(1, GetPoolOccupied("test", 1, 1));
        }

        [TestMethod]
        public void HasAccess_WithPoolOverMaxAllowed_ReturnsFalse()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            int poolId = InsertPool(1, 2, 1);
            InsertAccessControlListValue("test", 1, 1, serviceProfileId, poolId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1, 1);

            // Assert
            Assert.AreEqual(false, result);
            Assert.AreEqual(2, GetPoolOccupied("test", 1, 1));
        }

        [TestMethod]
        public void HasAccess_WithoutPool_Found_ReturnsTrue()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1, 1);

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void GetServiceProfile_7x24_ReturnsServiceProfileWithCorrectTimes()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("7x24");
            InsertTimeSlots(serviceProfileId, 00, 00, 24, 00);
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.GetServiceProfile("test", 1, 1);

            // Assert
            Assert.IsTrue(result.All(t => t.StartHour == 0));
            Assert.IsTrue(result.All(t => t.StartMinutes == 0));
            Assert.IsTrue(result.All(t => t.EndHour == 24));
            Assert.IsTrue(result.All(t => t.EndMinutes == 0));
            Assert.IsNotNull(result.Single(t => t.DayOfWeek == 0));
            Assert.IsNotNull(result.Single(t => t.DayOfWeek == 1));
            Assert.IsNotNull(result.Single(t => t.DayOfWeek == 2));
            Assert.IsNotNull(result.Single(t => t.DayOfWeek == 3));
            Assert.IsNotNull(result.Single(t => t.DayOfWeek == 4));
            Assert.IsNotNull(result.Single(t => t.DayOfWeek == 5));
            Assert.IsNotNull(result.Single(t => t.DayOfWeek == 6));
        }

        [TestMethod]
        public void GetServiceProfile_Night_ReturnsServiceProfileWithCorrectTimes()
        {
            // Arrange
            int serviceProfileId = InsertServiceProfile("Night");
            InsertTimeSlots(serviceProfileId, 17, 00, 24, 00);
            InsertTimeSlots(serviceProfileId, 00, 00, 05, 00);
            InsertAccessControlListValue("test", 1, 1, serviceProfileId);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.GetServiceProfile("test", 1, 1);

            // Assert
            Assert.AreEqual(7, result.Count(t => t.StartHour == 17));
            Assert.AreEqual(7, result.Count(t => t.StartHour == 00));
            Assert.IsTrue(result.All(t => t.StartMinutes == 0));
            Assert.AreEqual(7, result.Count(t => t.EndHour == 24));
            Assert.AreEqual(7, result.Count(t => t.EndHour == 05));
            Assert.IsTrue(result.All(t => t.EndMinutes == 0));
            Assert.AreEqual(2, result.Count(t => t.DayOfWeek == 0));
            Assert.AreEqual(2, result.Count(t => t.DayOfWeek == 1));
            Assert.AreEqual(2, result.Count(t => t.DayOfWeek == 2));
            Assert.AreEqual(2, result.Count(t => t.DayOfWeek == 3));
            Assert.AreEqual(2, result.Count(t => t.DayOfWeek == 4));
            Assert.AreEqual(2, result.Count(t => t.DayOfWeek == 5));
            Assert.AreEqual(2, result.Count(t => t.DayOfWeek == 6));
        }

        private static int GetPoolOccupied(string accessDeviceId, int accessDeviceType, int locationId)
        {
            _testDbConnection.Open();
            var command = new SqlCommand(string.Format(@"
select p.Occupied
from AccessControlList acl
join Pool p on p.Id = acl.PoolId
where acl.AccessDevice = '{0}'
and acl.AccessDeviceType = {1}
and acl.LocationId = {2}", accessDeviceId, accessDeviceType, locationId), _testDbConnection);
            var poolOccupied = (int)command.ExecuteScalar();
            _testDbConnection.Close();
            return poolOccupied;
        }

        private static int InsertPool(int maxAllowed, int occupied, int hard)
        {
            _testDbConnection.Open();
            var command = new SqlCommand(string.Format(@"
INSERT INTO [dbo].[Pool]
           ([MaxAllowed]
           ,[Occupied]
           ,[Hard])
     VALUES
           ({0}
           ,{1}
           ,{2});
SELECT CAST(scope_identity() AS int);
", maxAllowed, occupied, hard), _testDbConnection);
            var poolId = (int)command.ExecuteScalar();
            _testDbConnection.Close();
            return poolId;
        }

        private static int InsertServiceProfile(string serviceProfileName)
        {
            _testDbConnection.Open();
            var command = new SqlCommand(string.Format(@"
INSERT INTO [dbo].[ServiceProfile]
           ([Name])
     VALUES
           ('{0}');
SELECT CAST(scope_identity() AS int);
", serviceProfileName), _testDbConnection);
            var spId = (int)command.ExecuteScalar();
            _testDbConnection.Close();
            return spId;
        }

        private static void InsertTimeSlots(int serviceProfileId, int startHour, int startMinutes, int endHour, int endMinutes)
        {
            _testDbConnection.Open();
            for (int dayOfWeek = 0; dayOfWeek <= 6; dayOfWeek++)
            {
                var command = new SqlCommand(string.Format(@"
INSERT INTO [dbo].[TimeSlot]
           ([ServiceProfileId]
           ,[DayOfWeek]
           ,[StartHour]
           ,[StartMinutes]
           ,[EndHour]
           ,[EndMinutes])
     VALUES
           ({0}
           ,{1}
           ,{2}
           ,{3}
           ,{4}
           ,{5})
", serviceProfileId, dayOfWeek, startHour, startMinutes, endHour, endMinutes), _testDbConnection);
                command.ExecuteNonQuery();
            }
            _testDbConnection.Close();
        }

        private static void InsertAccessControlListValue(string accessDeviceId, int accessDeviceType, int locationId, int serviceProfileId, int? poolId = null)
        {
            _testDbConnection.Open();
            var command = new SqlCommand(string.Format(@"
INSERT INTO AccessControlList
		(AccessDevice
		,AccessDeviceType
        ,LocationId
		,ServiceProfileId
        ,PoolId)
	VALUES
		('{0}',{1}, {2}, {3}, {4})", accessDeviceId, accessDeviceType, locationId, serviceProfileId, poolId.HasValue?poolId.ToString():"NULL"), _testDbConnection);
            command.ExecuteNonQuery();
            _testDbConnection.Close();
        }
    }
}
