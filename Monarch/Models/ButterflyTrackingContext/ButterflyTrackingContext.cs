using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class ButterflyTrackingContext : DbContext
    {
        // this comment was added on another machine
        public ButterflyTrackingContext() : base("ButterflyTrackingContext") {
            Database.CommandTimeout = 200;
        }

        public DbSet<Butterfly> Butterflies { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<HumanSighting> HumanSightings { get; set; }
        public DbSet<MachineMonitor> MachineMonitors { get; set; }
        public DbSet<MachineSighting> MachineSightings { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PersonDetail> PersonDetails { get; set; }
    }
}