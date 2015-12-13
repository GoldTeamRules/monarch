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
    public class MonitorsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: Monitors
        public ActionResult Index()
        {
            var machineMonitors = db.MachineMonitors.Include(m => m.Organization).Include(m => m.UserFileUpload);
            return View(machineMonitors.ToList());
        }

        // GET: Monitors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.MachineMonitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // GET: Monitors/Create
        public ActionResult Create()
        {
            ViewBag.OrganizationId = new SelectList(db.Organizations, "OrganizationId", "UniqueName");
            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId");
            return View();
        }

        // POST: Monitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MonitorId,UserFileUploadId,OrganizationId,UniqueName,Latitude,Longitude,DisplayName,City,StateProvince,Country,PostalCode")] Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.MachineMonitors.Add(monitor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrganizationId = new SelectList(db.Organizations, "OrganizationId", "UniqueName", monitor.OrganizationId);
            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId", monitor.UserFileUploadId);
            return View(monitor);
        }

        // GET: Monitors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.MachineMonitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrganizationId = new SelectList(db.Organizations, "OrganizationId", "UniqueName", monitor.OrganizationId);
            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId", monitor.UserFileUploadId);
            return View(monitor);
        }

        // POST: Monitors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MonitorId,UserFileUploadId,OrganizationId,UniqueName,Latitude,Longitude,DisplayName,City,StateProvince,Country,PostalCode")] Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrganizationId = new SelectList(db.Organizations, "OrganizationId", "UniqueName", monitor.OrganizationId);
            ViewBag.UserFileUploadId = new SelectList(db.UserFileUploads, "UserFileUploadId", "UserFileUploadId", monitor.UserFileUploadId);
            return View(monitor);
        }

        // GET: Monitors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.MachineMonitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // POST: Monitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Monitor monitor = db.MachineMonitors.Find(id);
            db.MachineMonitors.Remove(monitor);
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
