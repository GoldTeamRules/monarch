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
    public class UserFileUploadsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: UserFileUploads
        public ActionResult Index()
        {
            return View(db.UserFileUploads.ToList());
        }

        // GET: UserFileUploads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFileUpload userFileUpload = db.UserFileUploads.Find(id);
            if (userFileUpload == null)
            {
                return HttpNotFound();
            }
            return View(userFileUpload);
        }

        // GET: UserFileUploads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserFileUploads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserFileUploadId,UserId,DateTime")] UserFileUpload userFileUpload)
        {
            if (ModelState.IsValid)
            {
                db.UserFileUploads.Add(userFileUpload);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userFileUpload);
        }

        // GET: UserFileUploads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFileUpload userFileUpload = db.UserFileUploads.Find(id);
            if (userFileUpload == null)
            {
                return HttpNotFound();
            }
            return View(userFileUpload);
        }

        // POST: UserFileUploads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserFileUploadId,UserId,DateTime")] UserFileUpload userFileUpload)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userFileUpload).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userFileUpload);
        }

        // GET: UserFileUploads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFileUpload userFileUpload = db.UserFileUploads.Find(id);
            if (userFileUpload == null)
            {
                return HttpNotFound();
            }
            return View(userFileUpload);
        }

        // POST: UserFileUploads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserFileUpload userFileUpload = db.UserFileUploads.Find(id);
            db.UserFileUploads.Remove(userFileUpload);
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
