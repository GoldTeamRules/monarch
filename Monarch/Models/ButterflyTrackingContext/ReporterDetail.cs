using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class ReporterDetail
    {
        public int ReporterDetailId { get; set; }
        public int ReporterId { get; set; }
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var details = obj as ReporterDetail;
            if (details == null)
                return false;

            // Return true if the fields match:
            return ReporterDetailId == details.ReporterDetailId;
        }

        // override hashcode to be just the reporter id (used for hashsets)
        public override int GetHashCode()
        {
            return ReporterDetailId;
        }
    }
}