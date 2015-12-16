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
using System.Data.Entity.Validation;
using System.Text;

namespace Monarch.Controllers
{
    public class UserFileUploadsController : Controller
    {
        private ButterflyTrackingContext db = new ButterflyTrackingContext();

        public ActionResult Index()
        {
            return View();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "UserFileUploadId,ReporterId,DateTime")] UserFileUpload userFileUpload, HttpPostedFileBase upload)
        {
            var errors = new List<string>();
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    // get the reporter entity linked the user for uploading the file
                    var reporterAttachedToUser = db.GetReporterFromUserId(User.Identity.GetUserId(), User.Identity.Name);
                    userFileUpload.Reporter = reporterAttachedToUser;
                    userFileUpload.ReporterId = reporterAttachedToUser.ReporterId;
                    // set the date of the user file upload to NOW
                    userFileUpload.DateTime = DateTime.Now;
                    db.UserFileUploads.Add(userFileUpload);
                    db.SaveChanges();

                    Regex digitsOnly = new Regex(@"[^\d]"); // this to remove non-numerical characters for phone numbers

                    double dummyDouble;
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
                                       stringToTest.ToUpper().Trim() == "R"
                                    || stringToTest.ToUpper().Trim() == "T"
                                    || stringToTest.ToUpper().Trim() == "M"
                                    || stringToTest.ToUpper().Trim() == "A"
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "Name",
                                length: 36,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
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
                                key: "StreetAddress",
                                length: 34,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "City",
                                length: 34,
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
                                key: "Country",
                                length: 30,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "PostalCode",
                                length: 14,
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
                                conformanceTest: stringToTest => true,
                                nullable: false
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "HomePhone",
                                length: 14,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                                // TODO: maybe add better conformance testing for phone numbers
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "CellPhone",
                                length: 14,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "Organization",
                                length: 30,
                                conversionFromStringToDataType: dataString => dataString,
                                conversionFromDataToString: data => data,
                                conformanceTest: stringToTest => true
                            )
                            // TOTAL RECORD LENGTH IS 296 CHARACTERS
                        }
                    );
                    usersFile.HeaderLead = "H";
                    usersFile.FooterLead = "T";

                    using (var reader = new StreamReader(upload.InputStream))
                    {
                        string errorMessage;

                        if (!usersFile.TryRead(reader, out errorMessage))
                        {
                            var error = new UserFileError
                            {
                                Error = "Could not parse users batch file. <b>Check the file and try again.</b>\n\n"
                                + "<pre>"
                                    + errorMessage + "\n"
                                + "</pre>",
                                UserFileUpload = userFileUpload
                            };
                            userFileUpload.Log = new List<UserFileError> { error };
                            db.SaveChanges();

                            return RedirectToAction("Index", "UserFileErrors", new { userFileUpload.UserFileUploadId });
                        }

                        try
                        {
                            userFileUpload.SequenceNumber = usersFile.Header.Sequence;
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            errors.Add("Could not get Sequence Number from header.\n " + e.Message);
                        }

                        var locationMaster = new LocationMaster();

                        int index = -1;
                        foreach(dynamic record in usersFile)
                        {
                            index++;
                            try
                            {
                                // USERS BATCH FILE TRANSFORMATION LOGIC
                                if (record.Type == "M") // if the record user type is 'M' for machine monitor
                                {
                                    var monitor = findMonitorFromUniqueName(record.UserName);
                                    if (monitor != null) // if there is already an existing monitor
                                    {
                                        errors.Add(string.Format(
                                            "Could not add record: [{0}] because monitor with UnquieName \'{1}\' aleady exists in the database",
                                            index,
                                            record.UserName));
                                        continue;
                                    }

                                    Organization organization = null;
                                    // identify organization name (if any)
                                    if (record.Organization != null)
                                    {
                                        organization = findOrganizationFromUniqueName(record.Organization);

                                        if (organization == null)
                                        {
                                            errors.Add(string.Format(
                                                "Could not add record: [{0}] because the record has an organization with UniqueName: \'{1}\' that does not exist. Please create this organization and try again.",
                                                index, record.Organization));
                                            continue;
                                        }
                                    }
                                    string message;
                                    if (!locationMaster.TryMasterLocation(record.Latitude , record.Longitude,
                                        record.City, record.State, null, out message))
                                    {
                                        errors.Add(string.Format("Could not add record: [{0}]: {1}", index, message));
                                        continue;
                                    }

                                    // ADD A NEW MONITOR WITH ALL THE MASTERED INFORMATION YAY!
                                    db.Monitors.Add(new Monitor
                                    {
                                        City = locationMaster.City,
                                        Country = locationMaster.Country,
                                        //DisplayName = record.Name,
                                        Latitude = locationMaster.Latitude,
                                        Longitude = locationMaster.Longitude,
                                        Organization = organization,
                                        PostalCode = locationMaster.PostalCode,
                                        StateProvince = locationMaster.State,
                                        UniqueName = record.UserName,
                                        UserFileUpload = userFileUpload
                                    });
                                    db.SaveChanges();
                                }
                                else // if record type is an R , T, or A (a person)
                                {
                                    var reporter = findReporterFromUsername(record.UserName);
                                    if (reporter != null)
                                    {
                                        errors.Add(string.Format(
                                            "Could not add record: [{0}] because username \'{1}\' already exists.",
                                            index, record.UserName));
                                    }

                                    Organization organization = null;
                                    // identify organization name (if any)
                                    if (record.Organization != null)
                                    {
                                        organization = findOrganizationFromUniqueName(record.Organization);

                                        if (organization == null)
                                        {
                                            errors.Add(string.Format(
                                                "Could not add record: [{0}] because the record has an organization with UniqueName: \'{1}\' that does not exist. Please create this organization and try again.",
                                                index, record.Organization));
                                            continue;
                                        }
                                    }

                                    var reporterToBeAdded = new Reporter
                                    {
                                        UserName = record.UserName,
                                        CellPhone = record.CellPhone,
                                        HomePhone = record.HomePhone,
                                        StreetAddress = record.StreetAddress,
                                        Organization = organization,
                                        Name = record.Name
                                    };

                                    db.Reporters.Add(reporterToBeAdded);
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
                            catch (Exception e) // might as well catch everything just in case
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

                var log = new List<UserFileError>();
                errors.ForEach(error => log.Add(new UserFileError { Error = error, UserFileUploadId = userFileUpload.UserFileUploadId }));
                userFileUpload.Log = log;
                log.ForEach(e => db.UserFileErrors.Add(e));
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Dispose();
                    db = null;
                    db = new ButterflyTrackingContext();
                    errors.ForEach(error => db.UserFileErrors.Add(
                        new UserFileError
                        {
                            Error = error,
                            UserFileUploadId = userFileUpload.UserFileUploadId
                        }));
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "UserFileErrors", new { userFileUpload.UserFileUploadId } );
            }

            return View(userFileUpload);
        }

        private Reporter findReporterFromUsername(string username)
        {
            var reporters = from r in db.Reporters
                            where r.UserName == username
                            select r;

            if (reporters.Count() <= 0)
            {
                return null;
            }

            if (reporters.Count() == 1)
            {
                return reporters.First();
            }

            if (reporters.Count() > 1)
            {
                throw new InvalidOperationException(string.Format(
                    "More than one reporter was found with username \'{0}\'. Please contact your database admin!",
                    username));
            }

            return null;
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
