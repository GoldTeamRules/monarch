using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class HumanSighting
    {
        public int HumanSightingId { get; set; }
        public int PersonId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public int FileUploadId { get; set; }

        public virtual Person Person { get; set; }
    }
}