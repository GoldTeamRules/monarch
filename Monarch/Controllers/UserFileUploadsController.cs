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
using SimpleFixedWidthParser;
using System.Text.RegularExpressions;
using Monarch.Models;
using System.IO;

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
        public ActionResult Create([Bind(Include = "UserFileUploadId,ReporterId,DateTime")] UserFileUpload userFileUpload, HttpPostedFileBase upload)
        {
            var log = new List<string>();
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    // get the reporter entity linked the user for uploading the file
                    var reporterAttachedToUser = db.GetReporterIdFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                    userFileUpload.Reporter = reporterAttachedToUser;
                    userFileUpload.ReporterId = reporterAttachedToUser.ReporterId;
                    // set the date of the sighting file upload to TODAY
                    userFileUpload.DateTime = DateTime.Today;
                    db.UserFileUploads.Add(userFileUpload);
                    db.SaveChanges();

                    Regex digitsOnly = new Regex(@"[^\d]"); // this to remove non-numerical characters for phone numbers


                    var usersFile = new FixedWidthParser
                    (
                        filePath: "users.txt",
                        columnDefintions: new List<dynamic> // note this list is defined by the dynamic keyword
                        {
                            new FixedWidthColumn<string>
                            (
                                key: "Type",
                                length: 2,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest =>
                                       stringToTest.ToUpper() == "R"
                                    || stringToTest.ToUpper() == "T"
                                    || stringToTest.ToUpper() == "M"
                                    || stringToTest.ToUpper() == "A"
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "Name",
                                length: 36,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "StreetAddress",
                                length: 34,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "City",
                                length: 35,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "State",
                                length: 30,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "PostalCode",
                                length: 13,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "UserName",
                                length: 30,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => stringToTest.Contains("@"),
                                nullable: false
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "HomePhone",
                                length: 12,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                                // TODO: maybe add better conformance testing for phone numbers
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "CellPhone",
                                length: 12,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "Organization",
                                length: 20,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                        }
                    );

                    using (var reader = new StreamReader(upload.InputStream))
                    {
                        string errorMessage;

                        if (!usersFile.TryRead(reader, out errorMessage))
                        {
                            log.Add("Could not parse users file. Check the file and try again.\n" + errorMessage);
                            throw new NotImplementedException("Couldn't read the batch file. TODO: add some way to handle this");
                        }

                        var locationMaster = new LocationMaster();

                        int index = 0;
                        foreach(dynamic record in usersFile)
                        {
                            try
                            {
                                // USERS BATCH FILE TRANSFORMATION LOGIC
                                if (record.Type == "M") // if the record user type is 'M' for machine monitor
                                {
                                    var monitor = findMonitorFromUniqueName(record.UserName);
                                    if (monitor != null) // if there is already an existing monitor
                                    {
                                        log.Add(string.Format(
                                            "Could not add record: [{0}] because monitor with UnquieName \'{1}\' aleady exists in the database",
                                            index,
                                            record.UserName));
                                        continue;
                                    }
                                    // identify organization name (if any)
                                    if (record.Organization != null)
                                    {
                                        var organization = findOrganizationFromUniqueName(record.Organization);

                                        if (organization == null)
                                        {
                                            log.Add(string.Format(
                                                "Could not add record: [{0}] because the record lists and Organization UniqueName: \'{1}\' that does exist.",
                                                record.Organization));
                                            continue;
                                        }
                                        
                                    }
                                }

                            }
                            catch (Exception e) // might as well catch everything just in case
                            {
                                log.Add(string.Format("Could not add record: [{0}]: {1}", index, e.Message));
                                continue;
                            }
                        }
                    }
                }
                
                return RedirectToAction("Index");
            }

            return View(userFileUpload);
        }

        private Organization findOrganizationFromUniqueName(string uniqueName)
        {
            var organizations = from o in db.Organizations
                                where o.UniqueName.ToLower() == uniqueName.ToLower()
                                select o;

            if (organizations.Count() <= 0)
            {
                return null;
            }

            if (organizations.Count() == 1)
            {
                return organizations.First();
            }

            if (organizations.Count() > 1)
            {
                throw new InvalidOperationException(string.Format(
                    "More than one organization with unique name \'{0}\' exists. Please contact your database admin",
                    uniqueName));
            }

            return null;
        }

        private Monitor findMonitorFromUniqueName(string uniqueName)
        {
            var monitors = from m in db.Monitors
                           where m.UniqueName.ToLower() == uniqueName.ToLower()
                           select m;

            if (monitors.Count() <= 0) // case where no monitors are found
            {
                return null; // HAPPY PATH
            }

            if (monitors.Count() == 1) // case where the monitor already exists
            {
                return monitors.First();
            }

            if (monitors.Count() > 1)
            {
                throw new InvalidOperationException(string.Format(
                    "More than one monitor with unique name \'{0}\' exists. Please contact your database admin",
                    uniqueName));
            }
            return null;
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
        public ActionResult Edit([Bind(Include = "UserFileUploadId,ReporterId,DateTime")] UserFileUpload userFileUpload)
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
