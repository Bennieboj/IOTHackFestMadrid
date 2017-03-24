--select count(Id) from AccessControlList
select acl.Id, acl.AccessDevice, acl.AccessDeviceType, acl.LocationId, sp.Name, acl.PoolId
from AccessControlList acl
join ServiceProfile sp on sp.Id = acl.ServiceProfileId
where acl.AccessDevice in ('GF-CP-51', 'H647KPE', 'PM-W8011', 'LK53ABY', 'SMC1735')

select p.Id, p.MaxAllowed, p.Occupied, p.Hard
from Pool p 
where p.Id = 1


--select sp.Name, t.DayOfWeek, t.StartHour, t.StartMinutes, t.EndHour, t.EndMinutes
--from ServiceProfile sp
--join Timeslot t on t.ServiceProfileId = sp.Id
