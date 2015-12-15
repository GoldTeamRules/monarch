using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monarch.Models.ButterflyTrackingContext
{
    public class SightingFileError
    {
        public int SightingFileErrorId { get; set; }
        public string Error { get; set; }

        public int SightingFileUploadId { get; set; }
        public virtual SightingFileUpload SightingFileUpload { get; set; }
    }
}