using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class Monitor
    {
        // PK and FKs
        public int MonitorId { get; set; }
        public int? UserFileUploadId { get; set; }
       // [DisplayName("OrganizationId")]
        public int? OrganizationId { get; set; }

        // required fields
        [Required]
        [Index(IsUnique = true), StringLength(500)]
        public string UniqueName { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }

        // optional attributes
        public string DisplayName { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        
        // linked entities
        public virtual Organization Organization { get; set; }
        public virtual UserFileUpload UserFileUpload { get; set; }
    }
}