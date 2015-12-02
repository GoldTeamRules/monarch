using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class FileUpload
    {
        public int FileUploadId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; }
    }
}