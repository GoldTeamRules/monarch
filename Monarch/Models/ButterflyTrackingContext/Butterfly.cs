using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class Butterfly
    {
        public int ButterflyId { get; set; }
        [Required]
        public int ReporterId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Species { get; set; }
        // FK
        public int? SightingFileUploadId { get; set; }

        // these do not represent the current location of the butterfly
        // but the location of the inital tagging
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }

        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        // date time of the tagging
        [Required]
        public DateTime DateTime { get; set; }

        // reference to the reporter
        public virtual Reporter Reporter { get; set; }
        // reference to the sighting file upload
        public virtual SightingFileUpload SightingFileUpload { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var butterfly = obj as Butterfly;
            if (butterfly == null)
                return false;

            // Return true if the fields match:
            return ButterflyId == butterfly.ButterflyId;
        }

        // override hashcode to be just the reporter id (used for hashsets)
        public override int GetHashCode()
        {
            return ButterflyId;
        }
    }
}