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
    public class UserFileErrorsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: SightingFileErrors
        public ActionResult Index(int? userFileUploadId)
        {
            var userFileError = db.UserFileErrors.Where(e => e.UserFileUploadId == userFileUploadId.Value);
            //var sightingFileError = db.SightingFileErrors.Include(s => s.SightingFileUpload);
            return View(userFileError.ToList());
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
