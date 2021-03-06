﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Monarch.Models.ButterflyTrackingContext
{
    public enum ReporterType { Reporter, Tagger, Admin }

    public class Reporter
    {
        public int ReporterId { get; set; }
        public string Name { get; set; }
        [Required]
        [Index(IsUnique = true), StringLength(500)]
        public string UserName { get; set; }

        public int? UserFileUploadId { get; set; }
        public Guid? UserId { get; set; }

        public int? OrganizationId { get; set; }

        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        [Required]
        public ReporterType ReporterType { get; set; }
        [Required]
        public bool IsConfigured { get; set; }

        public virtual Organization Organization { get; set; }
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
    }
}