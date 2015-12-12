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
        public DbSet<ReporterSighting> HumanSightings { get; set; }
        public DbSet<Monitor> MachineMonitors { get; set; }
        public DbSet<MonitorSighting> MachineSightings { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Reporter> Reporters { get; set; }
        public DbSet<ReporterDetail> ReporterDetails { get; set; }
    }
}