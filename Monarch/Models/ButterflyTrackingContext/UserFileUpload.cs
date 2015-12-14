using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class UserFileUpload
    {
        public int UserFileUploadId { get; set; }
        [Required]
        public int ReporterId { get; set; }
        [Required]
        public DateTime DateTime { get; set; }

        // reporter entity that uploaded that users.txt batch file
        public virtual Reporter Reporter { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var file = obj as UserFileUpload;
            if (file == null)
                return false;

            // Return true if the fields match:
            return UserFileUploadId == file.UserFileUploadId;
        }

        // override get hash code for because the equals override
        public override int GetHashCode()
        {
            return UserFileUploadId;
        }
    }
}