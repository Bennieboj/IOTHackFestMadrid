USE [IOTMadrid]
GO

CREATE TABLE [dbo].[AccessControlList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccessDevice] [varchar](255) NOT NULL,
	[AccessDeviceType] [int] NOT NULL,
	[LocationId] [int] NOT NULL DEFAULT ((1)),
	[ServiceProfileId] [int] NOT NULL DEFAULT ((1)),
 CONSTRAINT [ADADTypeLocationId] PRIMARY KEY NONCLUSTERED 
(
	[AccessDevice] ASC,
	[AccessDeviceType] ASC,
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO