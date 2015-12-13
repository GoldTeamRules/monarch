using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class Organization
    {
        public int OrganizationId { get; set; }
        /// <summary>
        /// The reporter that created and owns the Organization
        /// </summary>
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string LogoUrl { get; set; }

        public virtual Reporter Owner { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var details = obj as Organization;
            if (details == null)
                return false;

            // Return true if the fields match:
            return OrganizationId == details.OrganizationId;
        }

        // override hashcode to be just the reporter id (used for hashsets)
        public override int GetHashCode()
        {
            return OrganizationId;
        }
    }
}