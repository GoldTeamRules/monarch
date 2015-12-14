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
    public class MonitorSightingsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: MonitorSightings
        public ActionResult Index()
        {
            var machineSightings = db.MonitorSightings.Include(m => m.Butterfly).Include(m => m.Monitor).Include(m => m.SightingFileUpload);
            return View(machineSightings.ToList());
        }

        // GET: MonitorSightings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonitorSighting monitorSighting = db.MonitorSightings.Find(id);
            if (monitorSighting == null)
            {
                return HttpNotFound();
            }
            return View(monitorSighting);
        }

        // GET: MonitorSightings/Create
        public ActionResult Create()
        {
            ViewBag.ButterflyId = new SelectList(db.Butterflies, "ButterflyId", "Name");
            ViewBag.MonitorId = new SelectList(db.Monitors, "MonitorId", "UniqueName");
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId");
            return View();
        }

        // POST: MonitorSightings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MonitorSightingId,MonitorId,ButterflyId,SightingFileUploadId,DateTime,FileUploadId")] MonitorSighting monitorSighting)
        {
            if (ModelState.IsValid)
            {
                db.MonitorSightings.Add(monitorSighting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ButterflyId = new SelectList(db.Butterflies, "ButterflyId", "Name", monitorSighting.ButterflyId);
            ViewBag.MonitorId = new SelectList(db.Monitors, "MonitorId", "UniqueName", monitorSighting.MonitorId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", monitorSighting.SightingFileUploadId);
            return View(monitorSighting);
        }

        // GET: MonitorSightings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonitorSighting monitorSighting = db.MonitorSightings.Find(id);
            if (monitorSighting == null)
            {
                return HttpNotFound();
            }
            ViewBag.ButterflyId = new SelectList(db.Butterflies, "ButterflyId", "Name", monitorSighting.ButterflyId);
            ViewBag.MonitorId = new SelectList(db.Monitors, "MonitorId", "UniqueName", monitorSighting.MonitorId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", monitorSighting.SightingFileUploadId);
            return View(monitorSighting);
        }

        // POST: MonitorSightings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MonitorSightingId,MonitorId,ButterflyId,SightingFileUploadId,DateTime,FileUploadId")] MonitorSighting monitorSighting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitorSighting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ButterflyId = new SelectList(db.Butterflies, "ButterflyId", "Name", monitorSighting.ButterflyId);
            ViewBag.MonitorId = new SelectList(db.Monitors, "MonitorId", "UniqueName", monitorSighting.MonitorId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", monitorSighting.SightingFileUploadId);
            return View(monitorSighting);
        }

        // GET: MonitorSightings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonitorSighting monitorSighting = db.MonitorSightings.Find(id);
            if (monitorSighting == null)
            {
                return HttpNotFound();
            }
            return View(monitorSighting);
        }

        // POST: MonitorSightings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MonitorSighting monitorSighting = db.MonitorSightings.Find(id);
            db.MonitorSightings.Remove(monitorSighting);
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
