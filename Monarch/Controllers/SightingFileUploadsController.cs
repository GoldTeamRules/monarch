using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monarch.Models.ButterflyTrackingContext;
using System.IO;
using System.Text;
using SimpleFixedWidthParser;
using Monarch.Models;
using Microsoft.AspNet.Identity;

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
        public ActionResult Create([Bind(Include = "SightingFileUploadId")] SightingFileUpload sightingFileUpload, HttpPostedFileBase upload)
        {
            var log = new List<string>();
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    // get the reporter entity linked the user for uploading the file
                    var reporterAttachedToUser = db.GetReporterIdFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                    sightingFileUpload.Reporter = reporterAttachedToUser;
                    sightingFileUpload.ReporterId = reporterAttachedToUser.ReporterId;
                    // set the date of the sighting file upload to TODAY
                    sightingFileUpload.DateTime = DateTime.Today;
                    db.SightingFileUploads.Add(sightingFileUpload);
                    db.SaveChanges();



                    DateTime dummyDate;
                    double dummyDouble;
                    int dummyInt;

                    // create the FixedWithParser object for the sightings batch file
                    var sightingsFile = new FixedWidthParser
                    (
                        filePath: "sightings.txt",
                        columnDefintions: new List<dynamic> // note this list is defined by the dynamic keyword
                        {
                            new FixedWidthColumn<string>
                            (
                                key: "Event",
                                length: 2,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest =>
                                       stringToTest.Trim().ToUpper() == "S"
                                    || stringToTest.Trim().ToUpper() == "T"
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "UserNameOrReporterId",
                                length: 30,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => stringToTest.Contains("@")
                            ),
                            new FixedWidthColumn<DateTime>
                            (
                                key: "DateTime",
                                length: 19,
                                conversionFromStringToDataType: dataString => DateTime.Parse(dataString),
                                conversionFromDataToString: data => data.ToString(@"yyyy-MM-dd H:mm:ss "),
                                conformanceTest: stringToTest => DateTime.TryParse(stringToTest, out dummyDate),
                                nullable: false
                            ),
                            new FixedWidthColumn<double>
                            (
                                key: "Latitude",
                                length: 11,
                                conversionFromStringToDataType: dataString => double.Parse(dataString),
                                conversionFromDataToString: data => string.Format("{0:+000.000000;-000.000000}", data),
                                conformanceTest: stringToTest => double.TryParse(stringToTest, out dummyDouble)
                            ),
                            new FixedWidthColumn<double>
                            (
                                key: "Longitude",
                                length: 11,
                                conversionFromStringToDataType: dataString => double.Parse(dataString),
                                conversionFromDataToString: data => string.Format("{0:+000.000000;-000.000000}", data),
                                conformanceTest: stringToTest => double.TryParse(stringToTest, out dummyDouble)
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "City",
                                length: 35,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => " " + data,
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
                                key: "Country",
                                length: 30,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "Species",
                                length: 20,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<int>
                            (
                                key: "Tag",
                                length: 11,
                                conversionFromStringToDataType: dataString => int.Parse(dataString),
                                conversionFromDataToString: data => data.ToString(),
                                conformanceTest: stringToTest => int.TryParse(stringToTest, out dummyInt)
                            )
                        }
                    );

                    using (var reader = new StreamReader(upload.InputStream))
                    {
                        string errorMessage;

                        if (!sightingsFile.TryRead(reader, out errorMessage))
                        {
                            log.Add("Could not parse sightings file. Check the file and try again.\n" + errorMessage);
                            throw new NotImplementedException("Couldn't read the batch file. TODO: add some way to handle this");
                        }

                        var locationMaster = new LocationMaster();

                        int index = 0;
                        foreach (dynamic record in sightingsFile)
                        {
                            try
                            {
                                //  SIGHTINGS BATCH FILE TRANSFORMATION
                                if (record.DateTime > sightingsFile.Header.DateTime)
                                {
                                    log.Add(string.Format(
                                        "Could not add record: [{0}] because the record date is greater than the date in the header",
                                        index));
                                    continue; // go on to the next record
                                }
                                else if (record.Event.ToUpper() == "S")
                                {
                                    if (record.Tag.IsNull) // if the tag IS null, then it's a human
                                    {
                                        string message;
                                        // tries to find the reporter from the user name, if it can't it'll throw an error
                                        Reporter reporter = findReporterFromIdOrUserName(
                                            record.UserNameOrReporterId.ToString(), out message);
                                        if (reporter == null)
                                        {
                                            log.Add(string.Format("Could not add record: [{0}] {1}", index, message));
                                            continue; // go on to the next record
                                        }
                                        // master location then create new human sighting

                                        if (!locationMaster.TryMasterLocation(
                                            record.Latitude, record.Longitude,
                                            record.City, record.State, record.Country,
                                            out message))
                                        {
                                            log.Add(string.Format("Could not add record: [{0}] {1}", index, message));
                                            continue; // go on to the next record
                                        }


                                        // ADD NEW REPORTER SIGHTING!!! WE DID IT YAYAYAYAYAY!!!!
                                        db.ReporterSightings.Add(new ReporterSighting
                                        {
                                            City = locationMaster.City,
                                            Country = locationMaster.Country,
                                            DateTime = record.DateTime,
                                            Latitude = locationMaster.Latitude,
                                            Longitude = locationMaster.Longitude,
                                            PostalCode = locationMaster.PostalCode,
                                            Reporter = reporter,
                                            SightingFileUpload = sightingFileUpload,
                                            StateProvince = locationMaster.State
                                        });
                                        db.SaveChanges();

                                    }
                                    else // if the tag is NOT null then it's a monitor sighting
                                    {
                                        // check the butterfly id
                                        string message;
                                        var butterfly = findAndVerifyButterflyForMonitors(record.Tag, record.Species, out message);
                                        if (butterfly == null)
                                        {
                                            log.Add(string.Format("Could not add record [{0}] {1}", index, message));
                                            continue;
                                        }

                                        var monitor = findAndVerifyMonitor(record.UserNameOrReporterId, record.Latitude, record.Longitude, out message);
                                        if (monitor == null)
                                        {
                                            log.Add(string.Format("Could not add record [(0)] {1}", index, message));
                                            continue;
                                        }

                                        // WE ADDED A NEW SIGHTINGS ENTRY YAYEYAHAYAYYAY!!!
                                        db.MonitorSightings.Add(new MonitorSighting
                                        {
                                            Butterfly = butterfly,
                                            DateTime = record.DateTime,
                                            Monitor = monitor,
                                            SightingFileUpload = sightingFileUpload
                                        });
                                        db.SaveChanges();
                                    }
                                }
                                else if (record.Event.ToUpper() == "T")
                                {
                                    // check to see if butterfly tag exists in system (it shouldn't)
                                    var butterfly = db.Butterflies.Find(record.Tag); // this returns null if it can't find the element
                                    if (butterfly != null) // so if this value isn't null then we found a tag
                                    {
                                        log.Add(
                                            string.Format("Could not add record: [{0}] because the tag \'{1}\' already exists.",
                                                index, record.Tag));
                                    }

                                    // look for a tagger match
                                    string message;
                                    var tagger = findReporterFromIdOrUserName(record.UserNameOrReporterId, out message);
                                    if (tagger == null)
                                    {
                                        log.Add(
                                            string.Format("Could not add record: [{0}] could not find tagger: {1}",
                                                index, message));
                                    }

                                    // master the location
                                    if (!locationMaster.TryMasterLocation(
                                        record.Latitude, record.Longitude,
                                        record.City, record.State, record.Country,
                                        out message))
                                    {
                                        log.Add(string.Format("Could not add record: [{0}]. "
                                            + "Could not verify location: {1}", index, message));
                                    }

                                    db.Butterflies.Add(new Butterfly
                                    {
                                        City = locationMaster.City,
                                        Country = locationMaster.Country,
                                        DateTime = record.DateTime,
                                        Latitude = locationMaster.Latitude,
                                        Longitude = locationMaster.Longitude,
                                        PostalCode = locationMaster.PostalCode,
                                        Reporter = tagger,
                                        SightingFileUpload = sightingFileUpload,
                                        Species = record.Species,
                                        StateProvince = locationMaster.State
                                    });
                                    db.SaveChanges();
                                }
                                
                            }
                            catch (Exception e)
                            {
                                log.Add(string.Format("Could not add record: [{0}]: {1}", index, e.Message));
                                continue;
                            }
                            index++;
                        }
                    }

                }
                
                var fileContents = log.ToString();
                return RedirectToAction("Index");
            }

            return View(sightingFileUpload);
        }

        private Monitor findAndVerifyMonitor(string uniqueNameOrMonitorId, double latitude, double longitude, out string message)
        {
            int monitorId;
            Monitor monitor;

            if (int.TryParse(uniqueNameOrMonitorId, out monitorId))
            {
                monitor = db.Monitors.Find(monitorId);
                if (monitor == null) // if we didn't find a monitor
                {
                    message = string.Format("No monitor was found with Id: \'{1}\'", monitorId);
                    return null;
                }
            }
            else // cannot parse as int; now try to match unique name
            {
                var monitors = from m in db.Monitors
                                where m.UniqueName.ToLower() == uniqueNameOrMonitorId.ToLower()
                                select m;
                if (monitors.Count() <= 0) // case where no monitors are returned
                {
                    message = string.Format("No monitor was found with UserName: \'{1}\'", uniqueNameOrMonitorId);
                    return null;
                }
                else if (monitors.Count() > 1) // case where there's more than one 
                {
                    message = string.Format("ERROR: two reporters exist with UserName: \'{1}\'"
                        + "Contact your database administrator", uniqueNameOrMonitorId);
                    return null;
                }
                else
                {
                    monitor = monitors.First();
                    if (monitor == null)
                    {
                        message = string.Format("Monitor with UniqueName: \'{1}\' returned a null value.",
                          uniqueNameOrMonitorId);
                        return null;
                    }
                }
            }

            // check to see if latitude and longitude roughly match
            if (Math.Round(monitor.Latitude) == Math.Round(latitude)
                && Math.Round(monitor.Longitude) == Math.Round(longitude))
            {
                message = "";
                return monitor;
            }
            else
            {
                message = string.Format("Location of monitor \'{0}\' from database is: ({1},{2})"
                    + " and location or record: ({3},{4})", monitor.UniqueName, monitor.Latitude, monitor.Longitude, latitude, longitude);
                return null;
            }
        }

        private Butterfly findAndVerifyButterflyForMonitors(int butterflyId, string species, out string message)
        {
            var butterfly = db.Butterflies.Find(butterflyId);
            if (butterfly == null)
            {
                message = string.Format("The tag \'{0}\' does not exist."
                   + "Add the tag first, then you can add this record.", butterflyId);
                return null; // throw out record and move on
            }
            if (butterfly.Species.ToLower() != species.ToLower())
            {
                message = string.Format("The tag returned a butterfly with the Species \'{1}\' "
                    + "and the record contains the species {2}", butterfly.Species, species);
                return null; // throw out record and move on
            }
            message = "";
            return butterfly;
        }

        private Reporter findReporterFromIdOrUserName(string userNameOrReporterId, out string message)
        {
            int reporterId;
            Reporter reporter;

            if (int.TryParse(userNameOrReporterId, out reporterId))
            {
                reporter = db.Reporters.Find(reporterId);
                if (reporter == null) // if we didn't find a reporter
                {
                    message = string.Format("No reporter was found with Id: \'{1}\'", reporterId);
                    return null;
                }
            }
            else // cannot parse as int; now try to match user name
            {
                var reporters = from r in db.Reporters
                                where r.UserName.ToLower() == userNameOrReporterId.ToLower()
                                select r;
                if (reporters.Count() <= 0) // case where no reporters are returned
                {
                    message = string.Format("No reporter was found with UserName: \'{1}\'", userNameOrReporterId);
                    return null;
                }
                else if (reporters.Count() > 1) // case where there's more than one 
                {
                    message = string.Format("ERROR: two reporters exist with UserName: \'{1}\'"
                        + "Contact your database administrator", userNameOrReporterId);
                    return null;
                }
                else
                {
                    reporter = reporters.First();
                    if (reporter == null)
                    {
                        message = string.Format("Reporter with UserName: \'{1}\' returned a null value.",
                          userNameOrReporterId);
                        return null;
                    }
                }
            }
            message = "";
            return reporter;
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
        public ActionResult Edit([Bind(Include = "SightingFileUploadId,ReporterId,DateTime")] SightingFileUpload sightingFileUpload)
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
