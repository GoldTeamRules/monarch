using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class ReporterSighting
    {
        // PK and FKs
        public int ReporterSightingId { get; set; }
        [Required]
        public int ReporterId { get; set; }
        public int? SightingFileUploadId { get; set; }

        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public DateTime DateTime { get; set; }

        public string Specices { get; set; }

        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public virtual Reporter Reporter { get; set; }
        public virtual SightingFileUpload SightingFileUpload { get; set; }
    }
}