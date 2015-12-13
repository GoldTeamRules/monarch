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
    public class SightingFileUploadsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: SightingFileUploads
        public ActionResult Index()
        {
            return View(db.SightingFileUploads.ToList());
        }
        
        // GET: SightingFileUploads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SightingFileUpload sightingFileUpload = db.SightingFileUploads.Find(id);
            if (sightingFileUpload == null)
            {
                return HttpNotFound();
            }
            return View(sightingFileUpload);
        }

        // GET: SightingFileUploads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SightingFileUploads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SightingFileUploadId,UserId,DateTime")] SightingFileUpload sightingFileUpload, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                db.SightingFileUploads.Add(sightingFileUpload);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sightingFileUpload);
        }

        // GET: SightingFileUploads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SightingFileUpload sightingFileUpload = db.SightingFileUploads.Find(id);
            if (sightingFileUpload == null)
            {
                return HttpNotFound();
            }
            return View(sightingFileUpload);
        }

        // POST: SightingFileUploads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SightingFileUploadId,UserId,DateTime")] SightingFileUpload sightingFileUpload)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sightingFileUpload).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sightingFileUpload);
        }

        // GET: SightingFileUploads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SightingFileUpload sightingFileUpload = db.SightingFileUploads.Find(id);
            if (sightingFileUpload == null)
            {
                return HttpNotFound();
            }
            return View(sightingFileUpload);
        }

        // POST: SightingFileUploads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SightingFileUpload sightingFileUpload = db.SightingFileUploads.Find(id);
            db.SightingFileUploads.Remove(sightingFileUpload);
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
