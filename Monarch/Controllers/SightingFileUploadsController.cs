﻿using System;
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
        public ActionResult Create([Bind(Include = "SightingFileUploadId,UserId,DateTime")] SightingFileUpload sightingFileUpload, HttpPostedFileBase upload)
        {
            var sb = new StringBuilder();
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    DateTime dummyDate;
                    double dummyDouble;
                    int dummyInt;
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
                                       stringToTest.Trim().ToUpper().Equals("S")
                                    || stringToTest.Trim().ToUpper().Equals("T")
                            ),
                            new FixedWidthColumn<string>
                            (
                                key: "UserName",
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
                                conformanceTest: stringToTest => double.TryParse(stringToTest, out dummyDouble),
                                nullable: false
                            ),
                            new FixedWidthColumn<double>
                            (
                                key: "Longitude",
                                length: 11,
                                conversionFromStringToDataType: dataString => double.Parse(dataString),
                                conversionFromDataToString: data => string.Format("{0:+000.000000;-000.000000}", data),
                                conformanceTest: stringToTest => double.TryParse(stringToTest, out dummyDouble),
                                nullable: false
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
                            // logic if the file couldn't get parsed
                        }

                        var stringBuilder = new StringBuilder();
                        foreach (dynamic record in sightingsFile)
                        {
                            if (record.DateTime > sightingsFile.Header.DateTime)
                            {
                                stringBuilder.AppendLine("Could not add record {0} because"
                                    + "the record date is greater than the date in the header");
                                continue; // go on to the next record
                            }
                            else if (record.Event == "S")
                            {
                                if (record.Tag.IsNull)
                                {

                                }
                            }
                        }
                    }

                }
                db.SightingFileUploads.Add(sightingFileUpload);
                db.SaveChanges();
                var fileContents = sb.ToString();
                return RedirectToAction("Index");
            }

            return View(sightingFileUpload);
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
        public ActionResult Edit([Bind(Include = "SightingFileUploadId,UserId,DateTime")] SightingFileUpload sightingFileUpload)
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
