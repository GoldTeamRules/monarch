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
using System.Data.Entity.Validation;

namespace Monarch.Controllers
{
    public class SightingFileUploadsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        // GET: SightingFileUploads/Create
        public ActionResult Index()
        {
            return View();
        }

        // POST: SightingFileUploads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "SightingFileUploadId")] SightingFileUpload sightingFileUpload, HttpPostedFileBase upload)
        {
            var errors = new List<string>();

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    // get the reporter entity linked the user for uploading the file
                    var reporterAttachedToUser = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                    sightingFileUpload.Reporter = reporterAttachedToUser;
                    sightingFileUpload.ReporterId = reporterAttachedToUser.ReporterId;
                    // set the date of the sighting file upload to NOW
                    sightingFileUpload.DateTime = DateTime.Now;
                    db.SightingFileUploads.Add(sightingFileUpload);
                    db.SaveChanges();

                    DateTime dummyDate;
                    double dummyDouble;
                    //int dummyInt;

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
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<DateTime>
                            (
                                key: "DateTime",
                                length: 20,
                                conversionFromStringToDataType: dataString => DateTime.Parse(dataString),
                                conversionFromDataToString: data => data.ToString(@"yyyy-MM-dd H:mm:ss "),
                                conformanceTest: stringToTest => DateTime.TryParse(stringToTest, out dummyDate),
                                nullable: false
                            ),
                            new FixedWidthColumn<double>
                            (
                                key: "Latitude",
                                length: 12,
                                conversionFromStringToDataType: dataString => double.Parse(dataString),
                                conversionFromDataToString: data => string.Format("{0:+000.000000;-000.000000}", data),
                                conformanceTest: stringToTest => double.TryParse(stringToTest, out dummyDouble)
                            ),
                            new FixedWidthColumn<double>
                            (
                                key: "Longitude",
                                length: 12,
                                conversionFromStringToDataType: dataString => double.Parse(dataString),
                                conversionFromDataToString: data => string.Format("{0:+000.000000;-000.000000}", data),
                                conformanceTest: stringToTest => double.TryParse(stringToTest, out dummyDouble)
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "City",
                                length: 34,
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
                                length: 10,
                                conversionFromStringToDataType: dataString => int.Parse(dataString),
                                conversionFromDataToString: data => data.ToString(),
                                conformanceTest: stringToTest => { int dummyUint; return int.TryParse(stringToTest, out dummyUint); }
                            )
                            // TOTAL RECORD LENGTH IS 200 CHARACTERS
                        }
                    );

                    using (var reader = new StreamReader(upload.InputStream))
                    {
                        string errorMessage;

                        if (!sightingsFile.TryRead(reader, out errorMessage))
                        {
                            var error = new SightingFileError
                            {
                                Error = "Could not parse sightings batch file. <b>Check the file and try again.</b>\n\n"
                                + "<pre>"
                                    + errorMessage + "\n"
                                + "</pre>",
                                SightingFileUpload = sightingFileUpload
                            };
                            sightingFileUpload.Log = new List<SightingFileError> { error };
                            db.SaveChanges();

                            return RedirectToAction("Index", "SightingFileErrors", new { sightingFileUpload.SightingFileUploadId });

                        }

                        try
                        {
                            sightingFileUpload.SequenceNumber = sightingsFile.Header.Sequence;
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            errors.Add("Could not get Sequence Number from header.\n " +e.Message);
                        }
                        
                        var locationMaster = new LocationMaster();

                        int index = -1;
                        foreach (dynamic record in sightingsFile)
                        {
                            index++;
                            try
                            {
                                //  SIGHTINGS BATCH FILE TRANSFORMATION
                                if (record.DateTime > sightingsFile.Header.DateTime)
                                {
                                    errors.Add(string.Format(
                                        "Could not add record: [{0}] because the record date is greater than the date in the header",
                                        index));
                                    continue; // go on to the next record
                                }
                                else if (record.Event.ToUpper() == "S")
                                {
                                    if (record.Tag == null) // if the tag IS null, then it's a human
                                    {
                                        string message;
                                        // tries to find the reporter from the user name, if it can't it'll throw an error
                                        Reporter reporter = findReporterFromIdOrUserName(
                                            record.UserNameOrReporterId.ToString(), out message);
                                        if (reporter == null)
                                        {
                                            errors.Add(string.Format("Could not add record: [{0}] {1}", index, message));
                                            continue; // go on to the next record
                                        }
                                        // master location then create new human sighting

                                        if (!locationMaster.TryMasterLocation(
                                            record.Latitude, record.Longitude,
                                            record.City, record.State, record.Country,
                                            out message))
                                        {
                                            errors.Add(string.Format("Could not add record: [{0}] {1}", index, message));
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
                                            StateProvince = locationMaster.State,
                                            Specices = record.Species
                                        });
                                        db.SaveChanges();

                                    }
                                    else // if the tag is NOT null then it's a monitor sighting
                                    {
                                        // check the butterfly id
                                        string message;
                                        var butterfly = findAndVerifyButterflyForMonitors(record.Tag, record.Species, out message);
                                        if (butterfly == null) // couldn't find the butterfly id
                                        {
                                            errors.Add(string.Format("Could not add record [{0}] {1}", index, message));
                                            continue;
                                        }

                                        // verify the location
                                        
                                        
                                        

                                        var monitor = findAndVerifyMonitor(record.UserNameOrReporterId, out message);
                                        if (monitor == null) // couldn't find or verify the monitor
                                        {
                                            errors.Add(string.Format("Could not add record [{0}] {1}", index, message));
                                            continue;
                                        }
                                        int tag = record.Tag;
                                        // WE ADDED A NEW SIGHTINGS ENTRY YAYEYAHAYAYYAY!!!
                                        db.MonitorSightings.Add(new MonitorSighting
                                        {
                                            //ButterflyId = tag,
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
                                        errors.Add(
                                            string.Format("Could not add record: [{0}] because the tag \'{1}\' already exists.",
                                                index, record.Tag));
                                        continue;
                                    }

                                    // look for a tagger match
                                    string message;
                                    var tagger = findReporterFromIdOrUserName(record.UserNameOrReporterId, out message);
                                    if (tagger == null)
                                    {
                                        errors.Add(
                                            string.Format("Could not add record: [{0}] could not find tagger: {1}",
                                                index, message));
                                        continue;
                                    }

                                    // master the location
                                    if (!locationMaster.TryMasterLocation(
                                        record.Latitude, record.Longitude,
                                        record.City, record.State, record.Country,
                                        out message))
                                    {
                                        errors.Add(string.Format("Could not add record: [{0}]. "
                                            + "Could not verify location: {1}", index, message));
                                        continue;
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
                            catch (DbEntityValidationException e)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine(string.Format("Could not add record: [{0}]: ", index));
                                    
                                    sb.AppendLine(string.Format("Entity of type \'{0}\' in state \'{1}\' has the following validation errors:",
                                        e.EntityValidationErrors.First().Entry.Entity.GetType().Name, e.EntityValidationErrors.First().Entry.State));
                                        sb.AppendLine(string.Format("- Property: \'{0}\', Error: \'{1}\'",
                                            e.EntityValidationErrors.First().ValidationErrors.First().PropertyName,
                                            e.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage));
                                errors.Add(sb.ToString());
                                sb.Clear();
                                continue;
                            }
                            catch (Exception e)
                            {
                                var inner = "";
                                if (e.InnerException != null)
                                {
                                    inner = e.InnerException.Message;
                                }
                                errors.Add(string.Format("Could not add record: [{0}]: {1}\n{2}", index, e.Message, inner));
                                continue;
                            }
                        }
                    }
                }
                
                var log = new List<SightingFileError>();
                errors.ForEach(error => log.Add(new SightingFileError { Error = error, SightingFileUploadId = sightingFileUpload.SightingFileUploadId }));
                sightingFileUpload.Log = log;
                log.ForEach(e => db.SightingFileErrors.Add(e));
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Dispose();
                    db = null;
                    db = new ButterflyTrackingContext();
                    errors.ForEach(error => db.SightingFileErrors.Add(
                        new SightingFileError
                        {
                            Error = error + "\n " + e.Message,
                            SightingFileUploadId = sightingFileUpload.SightingFileUploadId
                        }));
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "SightingFileErrors", new { sightingFileUpload.SightingFileUploadId } );
            }

            return View(sightingFileUpload);
        }

        private Monitor findAndVerifyMonitor(string uniqueNameOrMonitorId, out string message)
        {
            int monitorId;
            Monitor monitor;

            if (int.TryParse(uniqueNameOrMonitorId, out monitorId))
            {
                monitor = db.Monitors.Find(monitorId);
                if (monitor == null) // if we didn't find a monitor
                {
                    message = string.Format("No monitor was found with Id: \'{0}\'", monitorId);
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
                    message = string.Format("No monitor was found with UserName: \'{0}\'", uniqueNameOrMonitorId);
                    return null;
                }
                else if (monitors.Count() > 1) // case where there's more than one 
                {
                    message = string.Format("ERROR: two reporters exist with UserName: \'{0}\'"
                        + "Contact your database administrator", uniqueNameOrMonitorId);
                    return null;
                }
                else
                {
                    monitor = monitors.First();
                    if (monitor == null)
                    {
                        message = string.Format("Monitor with UniqueName: \'{0}\' returned a null value.",
                          uniqueNameOrMonitorId);
                        return null;
                    }
                }
            }

            //// check to see if latitude and longitude roughly match
            //if (true)
            //{
                message = "";
                return monitor;
            //}
            //else
            //{
                
            //    message = string.Format("Location of monitor \'{0}\' from database is: ({1},{2})"
            //        + " and location or record: ({3},{4})", monitor.UniqueName, monitor.Latitude, monitor.Longitude);
            //    return null;
            //}
        }

        private Butterfly findAndVerifyButterflyForMonitors(int tag, string species, out string message)
        {
            var butterflies = db.Butterflies.Where(e => e.Tag == tag);
            if (butterflies.Count() == 0)
            {
                message = string.Format("The tag \'{0}\' does not exist."
                   + "Add the tag first, then you can add this record.", tag);
                return null; // throw out record and move on
            } else if (butterflies.Count() > 1)
            {
                message = string.Format("ERROR: two butterflies exist with Tag: \'{0}\'"
                        + "Contact your database administrator", tag);
                return null;
            }

            // set the butterfly
            var butterfly = butterflies.First();


            if (butterfly.Species.ToLower() != species.ToLower())
            {
                message = string.Format("The tag returned a butterfly with the Species \'{0}\' "
                    + "and the record contains the species {1}", butterfly.Species, species);
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
                    message = string.Format("No reporter was found with Id: \'{0}\'", reporterId);
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
                    message = string.Format("No reporter was found with UserName: \'{0}\'", userNameOrReporterId);
                    return null;
                }
                else if (reporters.Count() > 1) // case where there's more than one 
                {
                    message = string.Format("ERROR: two reporters exist with UserName: \'{0}\'"
                        + "Contact your database administrator", userNameOrReporterId);
                    return null;
                }
                else
                {
                    reporter = reporters.First();
                    if (reporter == null)
                    {
                        message = string.Format("Reporter with UserName: \'{0}\' returned a null value.",
                          userNameOrReporterId);
                        return null;
                    }
                }
            }
            message = "";
            return reporter;
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
