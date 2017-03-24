USE [IOTMadrid]
GO

--if OBJECT_ID('dbo.AccessControlList_Pool') is not null
--drop table AccessControlList_Pool
--if OBJECT_ID('dbo.Pool') is not null
--drop table Pool
--if OBJECT_ID('dbo.TimeSlot') is not null
--drop table TimeSlot
--if OBJECT_ID('dbo.AccessControlList') is not null
--drop table AccessControlList
--if OBJECT_ID('dbo.ServiceProfile') is not null
--drop table ServiceProfile


--CREATE TABLE [dbo].[ServiceProfile](
--	[Id] [int] IDENTITY(1,1) NOT NULL primary key,
--	[Name] varchar(10) NOT NULL
--)

--GO

--CREATE TABLE [dbo].[TimeSlot](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[ServiceProfileId] [int] NOT NULL,
--	[DayOfWeek] [int] NOT NULL DEFAULT (1),
--	[StartHour] [int] NOT NULL DEFAULT ((1)),
--	[StartMinutes] [int] NOT NULL DEFAULT ((1)),
--	[EndHour] [int] NOT NULL DEFAULT ((1)),
--	[EndMinutes] [int] NOT NULL DEFAULT ((1)),
--	FOREIGN KEY (ServiceProfileId) REFERENCES ServiceProfile(Id)
--)

--GO

--CREATE TABLE [dbo].[AccessControlList](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[AccessDevice] [varchar](255) NOT NULL,
--	[AccessDeviceType] [int] NOT NULL,
--	[LocationId] [int] NOT NULL DEFAULT (1),
--	[ServiceProfileId] [int] NOT NULL DEFAULT (1),
--	FOREIGN KEY (ServiceProfileId) REFERENCES ServiceProfile(Id),
-- CONSTRAINT [ADADTypeLocationId] PRIMARY KEY NONCLUSTERED 
--(
--	[AccessDevice] ASC,
--	[AccessDeviceType] ASC,
--	[LocationId] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
--)

--GO
--CREATE UNIQUE INDEX UXId_AccessControlList ON AccessControlList(Id)

--CREATE TABLE [dbo].[Pool](
--	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
--	[MaxAllowed] [int] NOT NULL,
--	[Occupied] [int] NOT NULL,
--	[Hard] [bit] NOT NULL
--)

