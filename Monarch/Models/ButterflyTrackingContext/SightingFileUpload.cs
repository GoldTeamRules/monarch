using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class SightingFileUpload
    {
        // PK
        public int SightingFileUploadId { get; set; }
        // FK for reporter ID
        [Required]
        public int ReporterId { get; set; }
        // the date and time the file was uploaded
        [Required]
        public DateTime DateTime { get; set; }

        // the reporter who uploaded the file
        public virtual Reporter Reporter { get; set; }

        public List<SightingFileError> Log { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var file = obj as SightingFileUpload;
            if (file == null)
                return false;

            // Return true if the fields match:
            return SightingFileUploadId == file.SightingFileUploadId;
        }

        // override get hash code for because the equals override
        public override int GetHashCode()
        {
            return SightingFileUploadId;
        }
    }
}