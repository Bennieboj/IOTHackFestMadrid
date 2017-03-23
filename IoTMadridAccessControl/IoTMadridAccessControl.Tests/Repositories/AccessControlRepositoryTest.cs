using System.Data.SqlClient;
using IoTMadridAccessControl.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace IoTMadridAccessControl.Tests.Repositories
{
    [TestClass]
    public class AccessControlRepositoryTest
    {
        private static readonly string _masterConnectionString = "Server=.;Initial Catalog=master;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;";
        private static readonly string _testDbConnectionString = "Server=.;Initial Catalog=TESTIOTMadrid;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;";
        //private static readonly string _masterConnectionString = "Server=tcp:iotmadrid.database.windows.net,1433;Initial Catalog=master;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=100;";
        //private static readonly string _testDbConnectionString = "Server=tcp:iotmadrid.database.windows.net,1433;Initial Catalog=TESTIOTMadrid;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;";
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
	[id] [int] identity(1,1) not null,
	[accessdevice] [varchar](255) not null,
	[accessdevicetype] [int] not null,
	[locationid] [int] not null default (1),
	[serviceprofileid] [int] not null default (1),
	foreign key (serviceprofileid) references serviceprofile(Id),
constraint [adadtypelocationid] primary key nonclustered 
(
	[accessdevice] asc,
	[accessdevicetype] asc,
	[locationid] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
);
", _testDbConnection);
            command2.ExecuteNonQuery();
            _testDbConnection.Close();
        }

        [TestMethod]
        public void GetByAccessDevice_NotFoundEmpty_ReturnsFalse()
        {
            // Arrange
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1, 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void GetByAccessDevice_NotFoundWrongRecord_Type_ReturnsFalse()
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
        public void GetByAccessDevice_NotFoundWrongRecord_Name_ReturnsFalse()
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
        public void GetByAccessDevice_NotFoundWrongRecord_Location_ReturnsFalse()
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
        public void GetByAccessDevice_Found_ReturnsTrue()
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

        private static int InsertServiceProfile(string serviceProfileName)
        {
            _testDbConnection.Open();
            var command = new SqlCommand(string.Format(@"
INSERT INTO [dbo].[ServiceProfile]
           ([Name])
     VALUES
           ('{0}')
", serviceProfileName), _testDbConnection);
            command.ExecuteNonQuery();

            var commandGetId = new SqlCommand(string.Format(@"
select Id from ServiceProfile where Name = '{0}'
", serviceProfileName), _testDbConnection);
            var spId = commandGetId.ExecuteScalar();
            _testDbConnection.Close();
            return (int)spId;
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

        private static void InsertAccessControlListValue(string accessDeviceId, int accessDeviceType, int locationId, int serviceProfileId)
        {
            _testDbConnection.Open();
            var command = new SqlCommand(string.Format(@"
INSERT INTO AccessControlList
		(AccessDevice
		,AccessDeviceType
        ,LocationId
		,ServiceProfileId)
	VALUES
		('{0}',{1}, {2}, {3})
", accessDeviceId, accessDeviceType, locationId, serviceProfileId), _testDbConnection);
            command.ExecuteNonQuery();
            _testDbConnection.Close();
        }
    }
}
