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
            return View(db.Reporters.ToList());
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
            try {
                Reporter reporter = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                if (reporter.Organization != null)
                {
                    if (string.IsNullOrEmpty(reporter.Organization.DisplayName))
                        ViewBag.OrganizationName = reporter.Organization.DisplayName;
                    else
                        ViewBag.OrganizationName = reporter.Organization.UniqueName;

                    ViewBag.OrganizationId = reporter.OrganizationId;
                }

                if (reporter == null)
                {
                    return HttpNotFound();
                }
                return View(reporter);
            }
            catch (Exception e)
            {
                // i just don't want to fail...
                return View();
            }
        }

        // GET: Reporters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reporters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReporterId,Name,UserName,UserFileUploadId,UserId,OrganizationId,ProfilePictureUrl,Bio,StreetAddress,City,StateProvince,Country,PostalCode,HomePhone,CellPhone,ReporterType,IsConfigured")] Reporter reporter)
        {
            if (ModelState.IsValid)
            {
                db.Reporters.Add(reporter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
            return View(reporter);
        }

        // GET: Reporters/Edit/5
        public ActionResult EditMe()
        {
            
            Reporter reporter = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
            if (reporter == null)
            {
                return HttpNotFound();
            }
            return View(reporter);
        }

        // POST: Reporters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMe([Bind(Include = "ReporterId,Name,OrganizationId,ProfilePictureUrl,Bio,StreetAddress,City,StateProvince,Country,PostalCode,HomePhone,CellPhone")] Reporter reporter)
        {
            var currentReporter = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
            currentReporter.Name = reporter.Name;
            currentReporter.Bio = reporter.Bio;
            currentReporter.CellPhone = reporter.CellPhone;
            currentReporter.Country = reporter.Country;
            currentReporter.City = reporter.City;
            currentReporter.HomePhone = reporter.HomePhone;
            currentReporter.IsConfigured = true;
            currentReporter.PostalCode = reporter.PostalCode;
            currentReporter.StateProvince = reporter.StateProvince;
            currentReporter.StreetAddress = reporter.StreetAddress;
            currentReporter.ProfilePictureUrl = reporter.ProfilePictureUrl;
            try
            {
                db.Entry(currentReporter).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                // oh well
            }
            return RedirectToAction("Me");
            //return View(reporter);
        }

        // POST: Reporters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReporterId,Name,UserName,UserFileUploadId,UserId,OrganizationId,ProfilePictureUrl,Bio,StreetAddress,City,StateProvince,Country,PostalCode,HomePhone,CellPhone,ReporterType,IsConfigured")] Reporter reporter)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(reporter).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    return View(reporter);
                }
            }
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
