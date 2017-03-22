USE [IOTMadrid]
GO

IF OBJECT_ID('[dbo].[AccessControlList]', 'U') IS NOT NULL
  DROP TABLE [dbo].[AccessControlList]
GO

CREATE TABLE [dbo].[AccessControlList](
	[Id] [int] IDENTITY NOT NULL,
	[AccessDevice] [varchar](255) NOT NULL,
	[AccessDeviceType] [int] NOT NULL,
 CONSTRAINT [ADADType] PRIMARY KEY NONCLUSTERED 
(
	[AccessDevice] ASC,
	[AccessDeviceType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
