using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class MachineSighting
    {
        public int MachineSightingId { get; set; }
        public int MachineMonitorId { get; set; }
        public int ButterflyId { get; set; }
        public DateTime DateTime { get; set; }
        public int FileUploadId { get; set; }

        public virtual MachineMonitor MachineMonitor { get; set; }
        public virtual Butterfly Butterfly { get; set; }
    }
}