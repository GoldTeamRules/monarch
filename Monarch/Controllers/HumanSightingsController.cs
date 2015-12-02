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
    public class HumanSightingsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: HumanSightings
        public ActionResult Index()
        {
            var humanSightings = db.HumanSightings.Include(h => h.Person);
            return View(humanSightings.ToList());
        }

        // GET: HumanSightings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HumanSighting humanSighting = db.HumanSightings.Find(id);
            if (humanSighting == null)
            {
                return HttpNotFound();
            }
            return View(humanSighting);
        }

        // GET: HumanSightings/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.People, "PersonId", "Name");
            return View();
        }

        // POST: HumanSightings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HumanSightingId,PersonId,Latitude,Longitude,StreetAddress,City,StateProvince,Country,PostalCode,FileUploadId")] HumanSighting humanSighting)
        {
            if (ModelState.IsValid)
            {
                db.HumanSightings.Add(humanSighting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonId = new SelectList(db.People, "PersonId", "Name", humanSighting.PersonId);
            return View(humanSighting);
        }

        // GET: HumanSightings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HumanSighting humanSighting = db.HumanSightings.Find(id);
            if (humanSighting == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList(db.People, "PersonId", "Name", humanSighting.PersonId);
            return View(humanSighting);
        }

        // POST: HumanSightings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HumanSightingId,PersonId,Latitude,Longitude,StreetAddress,City,StateProvince,Country,PostalCode,FileUploadId")] HumanSighting humanSighting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(humanSighting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonId = new SelectList(db.People, "PersonId", "Name", humanSighting.PersonId);
            return View(humanSighting);
        }

        // GET: HumanSightings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HumanSighting humanSighting = db.HumanSightings.Find(id);
            if (humanSighting == null)
            {
                return HttpNotFound();
            }
            return View(humanSighting);
        }

        // POST: HumanSightings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HumanSighting humanSighting = db.HumanSightings.Find(id);
            db.HumanSightings.Remove(humanSighting);
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
