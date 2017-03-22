DECLARE @RowCount INT

DECLARE @Random INT
DECLARE @Upper INT
DECLARE @Lower INT

SET @RowCount = 0
SET @Lower = 1
SET @Upper = 6

WHILE @RowCount < 90000000
BEGIN	
	
	SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)

	INSERT INTO AccessControlList
		(AccessDevice
		,AccessDeviceType)
	VALUES
		(CONVERT(varchar(255), NEWID())
		,@Random)

	SET @RowCount = @RowCount + 1
END


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
