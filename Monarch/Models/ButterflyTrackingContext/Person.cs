using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class Person
    {
        public int PersonId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }

        public virtual List<PersonDetail> Details { get; set; }
    }
}