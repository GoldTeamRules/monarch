using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class ButterflyTrackingInitializer : DropCreateDatabaseIfModelChanges<ButterflyTrackingContext>
    //public class ButterflyTrackingInitializer : DropCreateDatabaseAlways<ButterflyTrackingContext>
    {
        protected override void Seed(ButterflyTrackingContext context)
        {
            var organizations = new List<Organization>
            {
                new Organization { UniqueName="Gold", Description="Gold Team Rules", WebsiteUrl="ButterflyTracking.com" },
                new Organization { UniqueName="Ford Motor Co.", Description="Michigan-based car company", WebsiteUrl="Ford.com" },
                new Organization { UniqueName="Google", Description="Technology Company", WebsiteUrl="Google.com" }
            };
            organizations.ForEach(e => context.Organizations.Add(e));
            context.SaveChanges();

            var reporters = new List<Reporter>
            {
                new Reporter
                {
                    Name ="Justin Hoyt", Bio="I like butterflies",
                    StreetAddress="555 Washinton", City="Allen Park",
                    StateProvince="Michigan", Country ="USA", PostalCode="48101",
                    CellPhone="(555) 555-5555", HomePhone="(333) 333-3333",
                    Details=new List<ReporterDetail> { new ReporterDetail { Organization=organizations[0] } },
                },
                new Reporter
                {
                    Name ="Professor Steiner", Bio="Software engineering is a diciplined process",
                    StreetAddress="555 Washinton", City="Dearborn",
                    StateProvince="Michigan", Country ="USA", PostalCode="48126",
                    CellPhone="(555) 555-5555", HomePhone="(333) 333-3333",
                    Details=new List<ReporterDetail> { new ReporterDetail { Organization=organizations[1] } },
                },
                new Reporter
                {
                    Name ="Jill", Bio="I really like butterflies",
                    StreetAddress="555 Washinton", City="Some City",
                    StateProvince="I Hate test data", Country ="USA", PostalCode="48101",
                    CellPhone="(555) 555-5555", HomePhone="(333) 333-3333",
                    Details=new List<ReporterDetail> { new ReporterDetail { Organization=organizations[2] } },
                },
            };
            reporters.ForEach(e => context.Reporters.Add(e));
            context.SaveChanges();

            var humanSightings = new List<ReporterSighting>
            {
                new ReporterSighting
                {
                    Reporter=reporters[0], Latitude=42.3144, Longitude=83.2133
                },
                new ReporterSighting
                {
                    Reporter=reporters[2], Latitude=42.3144, Longitude=83.2133
                }
            };
            humanSightings.ForEach(e => context.HumanSightings.Add(e));
            context.SaveChanges();

            var machines = new List<Monitor>
            {
                new Monitor
                {
                    DisplayName ="MONITOR1", Latitude=42.3144, Longitude=83.2133,
                    City ="Dearborn", StateProvince="Michigan", Country="USA",
                    PostalCode="48120", Organization=organizations[0]
                },
                new Monitor
                {
                    DisplayName ="MONITOR2", Latitude=42.3144, Longitude=83.2133,
                    City ="Some other city", StateProvince="Michigan", Country="USA",
                    PostalCode="48120", Organization=organizations[2]
                }
            };
            machines.ForEach(e => context.MachineMonitors.Add(e));
            context.SaveChanges();

            var butterflies = new List<Butterfly>
            {
                new Butterfly
                {
                    Name="John the butterfly", Species ="Monarch", Latitude=15.2695, Longitude=-43.1515,
                    Reporter=reporters[0], DateTime=new DateTime(2015,12,2)
                },
                new Butterfly
                {
                    Name="Jill the butterfly", Species ="American Snout", Latitude=15.2695, Longitude=-43.1515,
                    Reporter=reporters[1], DateTime=new DateTime(2015,12,1)
                }
            };
            butterflies.ForEach(e => context.Butterflies.Add(e));
            context.SaveChanges();

            var machineSightings = new List<MonitorSighting>
            {
                new MonitorSighting
                {
                    Butterfly=butterflies[0], Monitor=machines[0], DateTime=new DateTime(2015,12,2)
                },
                new MonitorSighting
                {
                    Butterfly=butterflies[1], Monitor=machines[1], DateTime=new DateTime(2015,12,1)
                }
            };
            machineSightings.ForEach(e => context.MachineSightings.Add(e));
            context.SaveChanges();
        }
    }
}