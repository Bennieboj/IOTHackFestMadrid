using System.Data.SqlClient;
using IoTMadridAccessControl.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            var _testDbConnection = new SqlConnection(_testDbConnectionString);
            _testDbConnection.Open();
            var command2 = new SqlCommand(@"
CREATE TABLE [dbo].[AccessControlList](
	[Id] [int] IDENTITY NOT NULL,
	[AccessDevice] [varchar](255) NOT NULL,
	[AccessDeviceType] [int] NOT NULL,
 CONSTRAINT [ADADType] PRIMARY KEY CLUSTERED 
(
	[AccessDevice] ASC,
	[AccessDeviceType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

CREATE NONCLUSTERED INDEX [IX_AccessControlList] ON [AccessControlList]
(
	[AccessDevice], [AccessDeviceType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
", _testDbConnection);
            command2.ExecuteNonQuery();
            _testDbConnection.Close();
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
            _testDbConnection.Open();
            var command = new SqlCommand(@"
truncate table [AccessControlList];
", _testDbConnection);
            command.ExecuteNonQuery();
            _testDbConnection.Close();
        }

        [TestMethod]
        public void GetByAccessDevice_NotFoundEmpty_ReturnsFalse()
        {
            // Arrange
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void GetByAccessDevice_NotFoundWrongRecord_Type_ReturnsFalse()
        {
            // Arrange
            InsertValue("test", 2);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void GetByAccessDevice_NotFoundWrongRecord_Name_ReturnsFalse()
        {
            // Arrange
            InsertValue("test123", 1);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void GetByAccessDevice_Found_ReturnsTrue()
        {
            // Arrange
            InsertValue("test", 1);
            var repo = new AccessControlRepository(_testDbConnectionString);

            // Act
            var result = repo.HasAccess("test", 1);

            // Assert
            Assert.AreEqual(true, result);
        }

        private static void InsertValue(string accessDeviceId, int accessDeviceType)
        {
            _testDbConnection.Open();
            var command = new SqlCommand(string.Format(@"
INSERT INTO AccessControlList
		(AccessDevice
		,AccessDeviceType)
	VALUES
		('{0}'
		,{1})
", accessDeviceId, accessDeviceType), _testDbConnection);
            command.ExecuteNonQuery();
            _testDbConnection.Close();
        }
    }
}
