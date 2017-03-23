select acl.AccessDevice, acl.AccessDeviceType, acl.LocationId, sp.Name
from AccessControlList acl
join ServiceProfile sp on sp.Id = acl.ServiceProfileId
--join Timeslot t on t.ServiceProfileId = sp.Id
where acl.AccessDevice in ('GF-CP-51', 'H647KPE', 'PM-W8011', 'LK53ABY', 'SMC1735')

--select sp.Name, t.DayOfWeek, t.StartHour, t.StartMinutes, t.EndHour, t.EndMinutes
--from ServiceProfile sp
--join Timeslot t on t.ServiceProfileId = sp.Id
