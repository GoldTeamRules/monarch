using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monarch.Models.ButterflyTrackingContext;
using Microsoft.AspNet.Identity;
using Monarch.Models;

namespace Monarch.Controllers
{
    public class ReporterSightingsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: ReporterSightings
        public ActionResult Index()
        {
            var reporterSightings = db.ReporterSightings.Include(r => r.Reporter).Include(r => r.SightingFileUpload);
            return View(reporterSightings.ToList());
        }

        // GET: ReporterSightings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReporterSighting reporterSighting = db.ReporterSightings.Find(id);
            if (reporterSighting == null)
            {
                return HttpNotFound();
            }
            return View(reporterSighting);
        }

        // GET: ReporterSightings/Create
        public ActionResult Create()
        {
            ViewBag.ReporterId = new SelectList(db.Reporters, "ReporterId", "Name");
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId");
            return View();
        }

        // POST: ReporterSightings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReporterSightingId,SightingFileUploadId,Latitude,Longitude,DateTime,City,StateProvince,Country,PostalCode")] ReporterSighting reporterSighting)
        {
            try
            {
                var locationMaster = new LocationMaster();
                string message;
                if (locationMaster.TryMasterLocation(reporterSighting.Latitude, reporterSighting.Longitude,
                    reporterSighting.City, reporterSighting.StateProvince, reporterSighting.Country, out message))
                {
                    if (ModelState.IsValid)
                    {

                        ViewBag.Error = "";
                        reporterSighting.Latitude = locationMaster.Latitude;
                        reporterSighting.Longitude = locationMaster.Longitude;
                        reporterSighting.City = locationMaster.City;
                        reporterSighting.StateProvince = locationMaster.State;
                        reporterSighting.Country = locationMaster.Country;
                        //ModelState.Clear();

                        var user = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                        reporterSighting.Reporter = user;
                        reporterSighting.ReporterId = user.ReporterId;
                        db.Entry(reporterSighting).State = System.Data.Entity.EntityState.Added;
                        db.ReporterSightings.Add(reporterSighting);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = "";
                        reporterSighting.Latitude = locationMaster.Latitude;
                        reporterSighting.Longitude = locationMaster.Longitude;
                        reporterSighting.City = locationMaster.City;
                        reporterSighting.StateProvince = locationMaster.State;
                        reporterSighting.Country = locationMaster.Country;
                        ModelState.Clear();
                        //ViewBag.OrganizationId = new SelectList(db.Organizations, "OrganizationId", "UniqueName", reporterSighting.OrganizationId);
                        return View(reporterSighting);
                    }
                }
                else
                {
                    ViewBag.Error = message;
                    return View(reporterSighting);
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //ViewBag.OrganizationId = new SelectList(db.Organizations, "OrganizationId", "UniqueName", reporterSighting.OrganizationId);
                ViewBag.Exception = e.Message + " Are you missing a field? ";
                return View(reporterSighting);
            }
        }

        // GET: ReporterSightings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReporterSighting reporterSighting = db.ReporterSightings.Find(id);
            if (reporterSighting == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReporterId = new SelectList(db.Reporters, "ReporterId", "Name", reporterSighting.ReporterId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", reporterSighting.SightingFileUploadId);
            return View(reporterSighting);
        }

        // POST: ReporterSightings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReporterSightingId,ReporterId,SightingFileUploadId,Latitude,Longitude,DateTime,City,StateProvince,Country,PostalCode")] ReporterSighting reporterSighting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reporterSighting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReporterId = new SelectList(db.Reporters, "ReporterId", "Name", reporterSighting.ReporterId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", reporterSighting.SightingFileUploadId);
            return View(reporterSighting);
        }

        // GET: ReporterSightings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReporterSighting reporterSighting = db.ReporterSightings.Find(id);
            if (reporterSighting == null)
            {
                return HttpNotFound();
            }
            return View(reporterSighting);
        }

        // POST: ReporterSightings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReporterSighting reporterSighting = db.ReporterSightings.Find(id);
            db.ReporterSightings.Remove(reporterSighting);
            db.SaveChanges();
            return RedirectToAction("Index");
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
