using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class Butterfly
    {
        public int ButterflyId { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        // these do not represent the current location of the butterfly
        // but the location of the inital tagging
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        // date time of the tagging
        public DateTime DateTime { get; set; }
        public virtual Person Person { get; set; }
    }
}