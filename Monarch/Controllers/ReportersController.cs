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

namespace Monarch.Controllers
{
    public class ReportersController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: Reporters
        public ActionResult Index()
        {
            var reporters = db.Reporters.Include(r => r.UserFileUpload);
            return View(reporters.ToList());
        }

        // GET: Reporters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reporter reporter = db.Reporters.Find(id);
            if (reporter == null)
            {
                return HttpNotFound();
            }
            return View(reporter);
        }

        // GET: Reporters/Details/5
        public ActionResult Me()
        {
            Reporter reporter = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
            if (reporter == null)
            {
                return HttpNotFound();
            }
            return View(reporter);
        }

        // GET: Reporters/Create
        public ActionResult Create()
        {
            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId");
            return View();
        }

        // POST: Reporters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReporterId,Name,UserName,UserFileUploadId,ReporterId,ProfilePictureUrl,Bio,StreetAddress,City,StateProvince,Country,PostalCode,HomePhone,CellPhone")] Reporter reporter)
        {
            if (ModelState.IsValid)
            {
                db.Reporters.Add(reporter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId", reporter.UserFileUploadId);
            return View(reporter);
        }

        // GET: Reporters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reporter reporter = db.Reporters.Find(id);
            if (reporter == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId", reporter.UserFileUploadId);
            return View(reporter);
        }

        // GET: Reporters/EditMe
        public ActionResult EditMe()
        {
            Reporter reporter = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
            if (reporter == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(reporter);
        }

        // POST: Reporters/EditMe
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMe([Bind(Include = "ReporterId,Name,UserName,UserFileUploadId,ReporterId,ProfilePictureUrl,Bio,StreetAddress,City,StateProvince,Country,PostalCode,HomePhone,CellPhone")] Reporter reporter)
        {
            reporter = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
            if (ModelState.IsValid)
            {
                db.Entry(reporter).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Me");
            }
            //ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId", reporter.UserFileUploadId);
            return View(reporter);
        }

        // POST: Reporters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReporterId,Name,UserName,UserFileUploadId,ReporterId,ProfilePictureUrl,Bio,StreetAddress,City,StateProvince,Country,PostalCode,HomePhone,CellPhone")] Reporter reporter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reporter).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId", reporter.UserFileUploadId);
            return View(reporter);
        }

        // GET: Reporters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reporter reporter = db.Reporters.Find(id);
            if (reporter == null)
            {
                return HttpNotFound();
            }
            return View(reporter);
        }

        // POST: Reporters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reporter reporter = db.Reporters.Find(id);
            db.Reporters.Remove(reporter);
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
