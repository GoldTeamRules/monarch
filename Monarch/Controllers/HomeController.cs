using Monarch.Models.ButterflyTrackingContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Monarch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public ActionResult Delete()
        {
            
                var db = new ButterflyTrackingContext();

                var sets = new List<dynamic>()
                {
                    db.Butterflies,
                    db.Monitors,
                    db.MonitorSightings,
                    db.Organizations,
                    db.Reporters,
                    db.ReporterSightings,
                    db.SightingFileErrors,
                    db.SightingFileUploads,
                    db.UserFileErrors,
                    db.UserFileUploads
                };

            

                foreach(var set in sets)
                {
                    foreach(var entity in set)
                    {
                        set.Remove(entity);
                        db.SaveChanges();

                    }
                }
            //}
            //ViewBag.DeleteConfirmation = "All entities in the database were deleted.";
            return View();
        }

        

        public ActionResult About()
        {
            ViewBag.Message = "Gold Team Rules!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}