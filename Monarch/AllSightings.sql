select
    ReporterSightings.Latitude as "Latitude",
    ReporterSightings.Longitude as "Longitude",
	ReporterSightings.City as "City",
	ReporterSightings.StateProvince as "StateProvince",
	ReporterSightings.Country as "Country",
	ReporterSightings.PostalCode as "PostalCode",
    isnull(Reporters.Name, Reporters.Username) as "Name",
    'Human reporter sighting' as "Type",
    isnull(Organizations.Displayname, Organizations.UniqueName) as "Organization"
from ReporterSightings
inner join Reporters
    on ReporterSightings.ReporterId=Reporters.ReporterId
inner join Organizations
    on Reporters.OrganizationId=Organizations.OrganizationId

union


select
    Monitors.Latitude as "Latitude",
    Monitors.Longitude as "Longitude",
	Monitors.City as "City",
	Monitors.StateProvince as "StateProvince",
	Monitors.Country as "Country",
	Monitors.PostalCode as "PostalCode",
    isnull(Monitors.DisplayName, Monitors.UniqueName) as "Name",
    'Machine monitor sighting' as "Type",
    isnull(Organizations.Displayname, Organizations.UniqueName) as "Organization"
from MonitorSightings
inner join Monitors
    on MonitorSightings.MonitorId=Monitors.MonitorId
inner join Organizations
    on Monitors.OrganizationId=Organizations.OrganizationId