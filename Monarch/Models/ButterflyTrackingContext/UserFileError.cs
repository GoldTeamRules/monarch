using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class UserFileError
    {
        // PK
        public int UserFileErrorId { get; set; }
        // FK
        public int UserFileUploadId { get; set; }

        public string Error { get; set; }
        public virtual UserFileUpload UserFileUpload { get; set; }
    }
}