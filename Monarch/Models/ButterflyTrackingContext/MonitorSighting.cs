using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class MonitorSighting
    {
        public int MonitorSightingId { get; set; }

        [Required]
        public int MonitorId { get; set; }

        [Required]
        public int ButterflyId { get; set; }

        public int? SightingFileUploadId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        // linked objects
        public virtual Monitor Monitor { get; set; }
        public virtual Butterfly Butterfly { get; set; }
        public virtual SightingFileUpload SightingFileUpload { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var file = obj as MonitorSighting;
            if (file == null)
                return false;

            // Return true if the fields match:
            return MonitorSightingId == file.MonitorSightingId;
        }

        // override get hash code for because the equals override
        public override int GetHashCode()
        {
            return MonitorSightingId;
        }
    }
}