using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monarch.Models.ButterflyTrackingContext;

namespace Monarch.Controllers
{
    public class SightingFileErrorsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: SightingFileErrors
        public ActionResult Index(int? sightingFileUploadId)
        {
            var sightingFileError = db.SightingFileError.Where(e => e.SightingFileUploadId == sightingFileUploadId.Value);
            //var sightingFileError = db.SightingFileError.Include(s => s.SightingFileUpload);
            return View(sightingFileError.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
