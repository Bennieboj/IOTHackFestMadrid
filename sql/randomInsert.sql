--DECLARE @RowCount INT

--DECLARE @Random INT
--DECLARE @Upper INT
--DECLARE @Lower INT

--SET @RowCount = 0
--SET @Lower = 1
--SET @Upper = 6

--WHILE @RowCount < 90000000
--BEGIN	
	
--	SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)

--	INSERT INTO AccessControlList
--		(AccessDevice
--		,AccessDeviceType
--		,LocationId
--		,ServiceProfileId)
--	VALUES
--		(CONVERT(varchar(255), NEWID())
--		,@Random
--		,1
--		,1)

--	SET @RowCount = @RowCount + 1
--END
------- ^I moved this to a bulkinsert in the code, see BulkInsert folder



--INSERT INTO AccessControlList
--		(AccessDevice
--		,AccessDeviceType)
--	VALUES
--		('GF-CP-51'
--		,1)
--INSERT INTO AccessControlList
--		(AccessDevice
--		,AccessDeviceType)
--	VALUES
--		('H647KPE'
--		,1)
--INSERT INTO AccessControlList
--		(AccessDevice
--		,AccessDeviceType)
--	VALUES
--		('PM-W8011'
--		,1)
--INSERT INTO AccessControlList
--		(AccessDevice
--		,AccessDeviceType)
--	VALUES
--		('LK53ABY'
--		,1)
--INSERT INTO AccessControlList
--		(AccessDevice
--		,AccessDeviceType)
--	VALUES
--		('SMC1735'
--		,1)

--select top 10 * from AccessControlList



--INSERT INTO [dbo].[ServiceProfile]
--           ([Name])
--     VALUES
--           ('24x7')

--INSERT INTO [dbo].[ServiceProfile]
--           ([Name])
--     VALUES
--           ('Night')
--GO

--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (1, 0, 0, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (1, 1, 0, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (1, 2, 0, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (1, 3, 0, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (1, 4, 0, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (1, 5, 0, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (1, 6, 0, 0, 24, 0)

--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 0, 17, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 0, 24, 0, 5, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 1, 17, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 1, 24, 0, 5, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 2, 17, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 2, 24, 0, 5, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 3, 17, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 3, 24, 0, 5, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 4, 17, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 4, 24, 0, 5, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 5, 17, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 5, 24, 0, 5, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 6, 17, 0, 24, 0)
--INSERT INTO [dbo].[TimeSlot] ([ServiceProfileId], [DayOfWeek] ,[StartHour] ,[StartMinutes] ,[EndHour] ,[EndMinutes])
--VALUES (2, 6, 24, 0, 5, 0)


--INSERT INTO [dbo].[Pool]
--           ([MaxAllowed]
--           ,[Occupied]
--           ,[Hard])
--     VALUES
--           (1
--           ,0
--           ,1)
--GO

--Update AccessControlList
--set PoolId = 1
--where AccessDevice in('H647KPE', 'PM-W8011')

