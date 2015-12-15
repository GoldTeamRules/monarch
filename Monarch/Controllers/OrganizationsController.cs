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
using System.Data.Entity.Validation;
using System.Text;

namespace Monarch.Controllers
{
    public class OrganizationsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: Organizations
        public ActionResult Index()
        {
            return View(db.Organizations.ToList());
        }

        // GET: Organizations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // this would be to join an organization
        // POST: Organizations/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Organization organization)
        {

            Reporter reporter = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
            if (organization != null)
            {
                try
                {

                
                //reporter.Organization = organization;
                reporter.OrganizationId = organization.OrganizationId;
                db.Entry(reporter).State = System.Data.Entity.EntityState.Modified;
                db.Entry(organization).State = System.Data.Entity.EntityState.Unchanged;
                db.SaveChanges();
                return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
            
            
            return View(organization);
        }

        // GET: Organizations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrganizationId,OwnerId,UniqueName,DisplayName,Description,WebsiteUrl,LogoUrl")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Reporter ownwer = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                    organization.Owner = ownwer;
                    organization.OwnerId = ownwer.ReporterId;
                    db.Organizations.Add(organization);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    return View(organization);
                }
            }
            return View(organization);
        }

        // GET: Organizations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrganizationId,UniqueName,DisplayName,Description,WebsiteUrl,LogoUrl")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Reporter ownwer = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                    organization.Owner = ownwer;
                    organization.OwnerId = ownwer.ReporterId;
                    db.Entry(organization).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    return View(organization);
                }
            }
            return View(organization);
        }

        // GET: Organizations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Organization organization = db.Organizations.Find(id);
            db.Organizations.Remove(organization);
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
