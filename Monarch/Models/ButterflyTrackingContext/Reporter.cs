using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class Reporter
    {
        public int ReporterId { get; set; }
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }

        public int? UserFileUploadId { get; set; }
        public Guid? UserId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }

        public virtual List<ReporterDetail> Details { get; set; }
        public virtual UserFileUpload UserFileUpload { get;set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var reporter = obj as Reporter;
            if (reporter == null)
                return false;

            // Return true if the fields match:
            return ReporterId == reporter.ReporterId;
        }

        // override hashcode to be just the reporter id (used for hashsets)
        public override int GetHashCode()
        {
            return ReporterId;
        }

        public static Guid? GetUserId(int reporterId)
        {
            var db = new ButterflyTrackingContext();
            var matchingUsers = from user in db.Reporters
                         where user.ReporterId.Equals(reporterId)
                         select user;
            if (matchingUsers.Count() > 1)
            {
                throw new ApplicationException("ERROR: there were multiple matches for the given reporterId");
            }
            else if (matchingUsers.Count() <= 0)
            {
                return null;
            }
            return matchingUsers.First().UserId;
        }

        public static int? GetReporterId(Guid userId)
        {
            var db = new ButterflyTrackingContext();
            var matchingUsers = from user in db.Reporters
                                where user.UserId.Equals(userId)
                                select user;
            if (matchingUsers.Count() > 1)
            {
                throw new ApplicationException("ERROR: there were multiple matches for the given UserId");
            }
            else if (matchingUsers.Count() <= 0)
            {
                return null;
            }
            return matchingUsers.First().ReporterId;
        }
    }
}