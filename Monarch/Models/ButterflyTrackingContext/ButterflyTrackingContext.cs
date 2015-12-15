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
        public DbSet<SightingFileError> SightingFileErrors { get; set; }
        public DbSet<UserFileError> UserFileErrors { get; set; }
        //public DbSet<ReporterDetail> ReporterDetails { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<SightingFileUpload>().HasRequired(i => i.Reporter).WithOptional().WillCascadeOnDelete(false);
            modelBuilder.Entity<Reporter>().HasOptional(i => i.UserFileUpload).WithOptionalPrincipal();
            //modelBuilder.Entity<SightingFileUpload>().HasMany(i => i.Log).;
        }

        public Reporter GetReporterFromUserId(string userId, string userName)
        {
            return GetReporterFromUserId(new Guid(userId), userName);
        }

        public Reporter GetReporterFromUserId(Guid userId, string userName)
        {
            var reporterQueryByUserId = from r in Reporters
                           where r.UserId == userId
                           select r;

            if (reporterQueryByUserId.Count() > 1)
            {
                throw new InvalidOperationException("The query to get the reporter Id  from a " 
                    + "ReporterId Guid returned more than one reporter. Contact your database admin!");
            }

            // happy path
            if (reporterQueryByUserId.Count() == 1)
            {
                return reporterQueryByUserId.First();
            }

            // case where no reporters by userId have been found
            if (reporterQueryByUserId.Count() <= 0)
            {
                // check to see if there is a match on username
                var reporterQueryByUserName = from r in Reporters
                                              where r.UserName == userName
                                              select r;

                // check to see if there are more than one reporter per username
                if (reporterQueryByUserName.Count() > 1)
                {
                    // there should never be more than one so we'll just throw
                    throw new InvalidOperationException("The query to get the reporter Id from a "
                    + "UserName \'" + userName + "\' returned more than one reporter. Contact your database admin!");
                }

                if (reporterQueryByUserName.Count() == 1)
                {
                    // we found a reporter with the same user name
                    // this would be the case when the batch file created a user
                    // This means that the AspNetUser has not been linked to this account yet.
                    // SO... We'll link these now
                    var reporter = reporterQueryByUserName.First();
                    reporter.UserId = userId;
                    SaveChanges();
                    return reporter;
                }

                //no reporters by username have been found
                if (reporterQueryByUserName.Count() <= 0)
                {
                    // so we'll have to create a new reporter
                    var reporter = new Reporter
                    {
                        UserId = userId,
                        UserName = userName,
                        ReporterType = ReporterType.Reporter,
                        IsConfigured = false
                    };
                    Reporters.Add(reporter);
                    SaveChanges();
                    return reporter;
                }
            }
            return null;
        }
    }
}