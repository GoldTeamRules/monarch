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

        // GET: SightingFileErrors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SightingFileError sightingFileError = db.SightingFileError.Find(id);
            if (sightingFileError == null)
            {
                return HttpNotFound();
            }
            return View(sightingFileError);
        }

        // GET: SightingFileErrors/Create
        public ActionResult Create()
        {
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId");
            return View();
        }

        // POST: SightingFileErrors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SightingFileErrorId,Error,SightingFileUploadId")] SightingFileError sightingFileError)
        {
            if (ModelState.IsValid)
            {
                db.SightingFileError.Add(sightingFileError);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", sightingFileError.SightingFileUploadId);
            return View(sightingFileError);
        }

        // GET: SightingFileErrors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SightingFileError sightingFileError = db.SightingFileError.Find(id);
            if (sightingFileError == null)
            {
                return HttpNotFound();
            }
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", sightingFileError.SightingFileUploadId);
            return View(sightingFileError);
        }

        // POST: SightingFileErrors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SightingFileErrorId,Error,SightingFileUploadId")] SightingFileError sightingFileError)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sightingFileError).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SightingFileUploadId = new SelectList(db.SightingFileUploads, "SightingFileUploadId", "SightingFileUploadId", sightingFileError.SightingFileUploadId);
            return View(sightingFileError);
        }

        // GET: SightingFileErrors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SightingFileError sightingFileError = db.SightingFileError.Find(id);
            if (sightingFileError == null)
            {
                return HttpNotFound();
            }
            return View(sightingFileError);
        }

        // POST: SightingFileErrors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SightingFileError sightingFileError = db.SightingFileError.Find(id);
            db.SightingFileError.Remove(sightingFileError);
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
