using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class ButterflyTrackingInitializer : DropCreateDatabaseIfModelChanges<ButterflyTrackingContext>
    //public class ButterflyTrackingInitializer : DropCreateDatabaseAlways<ButterflyTrackingContext>
    {
        protected override void Seed(ButterflyTrackingContext context)
        {
        }
    }
}