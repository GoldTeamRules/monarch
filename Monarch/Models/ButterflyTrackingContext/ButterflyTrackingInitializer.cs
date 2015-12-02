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
                new Organization { Name="Gold", Description="Gold Team Rules", Website="ButterflyTracking.com" },
                new Organization { Name="Ford Motor Co.", Description="Michigan-based car company", Website="Ford.com" },
                new Organization { Name="Google", Description="Technology Company", Website="Google.com" }
            };
            organizations.ForEach(e => context.Organizations.Add(e));
            context.SaveChanges();

            var people = new List<Person>
            {
                new Person
                {
                    Name ="Justin Hoyt", Bio="I like butterflies",
                    StreetAddress="555 Washinton", City="Allen Park",
                    StateProvince="Michigan", Country ="USA", PostalCode="48101",
                    CellPhone="(555) 555-5555", HomePhone="(333) 333-3333",
                    Details=new List<PersonDetail> { new PersonDetail { Organization=organizations[0] } },
                },
                new Person
                {
                    Name ="Professor Steiner", Bio="Software engineering is a diciplined process",
                    StreetAddress="555 Washinton", City="Dearborn",
                    StateProvince="Michigan", Country ="USA", PostalCode="48126",
                    CellPhone="(555) 555-5555", HomePhone="(333) 333-3333",
                    Details=new List<PersonDetail> { new PersonDetail { Organization=organizations[1] } },
                },
                new Person
                {
                    Name ="Jill", Bio="I really like butterflies",
                    StreetAddress="555 Washinton", City="Some City",
                    StateProvince="I Hate test data", Country ="USA", PostalCode="48101",
                    CellPhone="(555) 555-5555", HomePhone="(333) 333-3333",
                    Details=new List<PersonDetail> { new PersonDetail { Organization=organizations[2] } },
                },
            };
            people.ForEach(e => context.People.Add(e));
            context.SaveChanges();

            var humanSightings = new List<HumanSighting>
            {
                new HumanSighting
                {
                    Person=people[0], Latitude=42.3144, Longitude=83.2133
                },
                new HumanSighting
                {
                    Person=people[2], Latitude=42.3144, Longitude=83.2133
                }
            };
            humanSightings.ForEach(e => context.HumanSightings.Add(e));
            context.SaveChanges();

            var machines = new List<MachineMonitor>
            {
                new MachineMonitor
                {
                    Name ="MONITOR1", Latitude=42.3144, Longitude=83.2133,
                    City ="Dearborn", StateProvince="Michigan", Country="USA",
                    PostalCode="48120", Organization=organizations[0]
                },
                new MachineMonitor
                {
                    Name ="MONITOR2", Latitude=42.3144, Longitude=83.2133,
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
                    Person=people[0], DateTime=new DateTime(2015,12,2)
                },
                new Butterfly
                {
                    Name="Jill the butterfly", Species ="American Snout", Latitude=15.2695, Longitude=-43.1515,
                    Person=people[1], DateTime=new DateTime(2015,12,1)
                }
            };
            butterflies.ForEach(e => context.Butterflies.Add(e));
            context.SaveChanges();

            var machineSightings = new List<MachineSighting>
            {
                new MachineSighting
                {
                    Butterfly=butterflies[0], MachineMonitor=machines[0], DateTime=new DateTime(2015,12,2)
                },
                new MachineSighting
                {
                    Butterfly=butterflies[1], MachineMonitor=machines[1], DateTime=new DateTime(2015,12,1)
                }
            };
            machineSightings.ForEach(e => context.MachineSightings.Add(e));
            context.SaveChanges();
        }
    }
}