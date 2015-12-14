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
    public class ButterfliesController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: Butterflies
        public ActionResult Index()
        {
            var butterflies = db.Butterflies.Include(b => b.Reporter).Include(b => b.SightingFileUpload);
            return View(butterflies.ToList());
        }

        // GET: Butterflies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Butterfly butterfly = db.Butterflies.Find(id);
            if (butterfly == null)
            {
                return HttpNotFound();
            }
            return View(butterfly);
        }

        // GET: Butterflies/Create
        public ActionResult Create()
        {
            ViewBag.ReporterId = new SelectList(db.Reporters, "ReporterId", "Name");
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId");
            return View();
        }

        // POST: Butterflies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ButterflyId,ReporterId,Species,SightingFileUploadId,Latitude,Longitude,City,StateProvince,Country,PostalCode,DateTime")] Butterfly butterfly)
        {
            if (ModelState.IsValid)
            {
                db.Butterflies.Add(butterfly);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReporterId = new SelectList(db.Reporters, "ReporterId", "Name", butterfly.ReporterId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", butterfly.SightingFileUploadId);
            return View(butterfly);
        }

        // GET: Butterflies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Butterfly butterfly = db.Butterflies.Find(id);
            if (butterfly == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReporterId = new SelectList(db.Reporters, "ReporterId", "Name", butterfly.ReporterId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", butterfly.SightingFileUploadId);
            return View(butterfly);
        }

        // POST: Butterflies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ButterflyId,ReporterId,Species,SightingFileUploadId,Latitude,Longitude,City,StateProvince,Country,PostalCode,DateTime")] Butterfly butterfly)
        {
            if (ModelState.IsValid)
            {
                db.Entry(butterfly).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReporterId = new SelectList(db.Reporters, "ReporterId", "Name", butterfly.ReporterId);
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", butterfly.SightingFileUploadId);
            return View(butterfly);
        }

        // GET: Butterflies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Butterfly butterfly = db.Butterflies.Find(id);
            if (butterfly == null)
            {
                return HttpNotFound();
            }
            return View(butterfly);
        }

        // POST: Butterflies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Butterfly butterfly = db.Butterflies.Find(id);
            db.Butterflies.Remove(butterfly);
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
