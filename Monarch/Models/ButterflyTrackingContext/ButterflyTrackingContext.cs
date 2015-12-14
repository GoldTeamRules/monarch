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
        public DbSet<SightingFileUpload> SightingFileUploads { get; set; }
        public DbSet<UserFileUpload> UserFileUploads { get; set; }
        public DbSet<ReporterSighting> ReporterSightings { get; set; }
        public DbSet<Monitor> Monitors { get; set; }
        public DbSet<MonitorSighting> MonitorSightings { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Reporter> Reporters { get; set; }
        public DbSet<ReporterDetail> ReporterDetails { get; set; }
    }
}