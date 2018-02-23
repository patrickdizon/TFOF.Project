using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TFOF.Areas.PDF.Models;
using TFOF.Areas.Core.Controllers;
using System.IO;
using TFOF.Areas.Core.Services;

namespace TFOF.Areas.PDF.Controllers
{
    public class PDFController : CoreController
    {
        private PDFModelContext db = new PDFModelContext();

        // GET: PDF/PDFModel
        public ActionResult Index()
        {
            return View(db.PDFs.ToList());
        }

        // GET: PDF/PDFModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PDFModel pdfModel = db.PDFs.Find(id);
            if (pdfModel == null)
            {
                return HttpNotFound();
            }
            return View(pdfModel);
        }

        // GET: PDF/PDFModel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PDF/PDFModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HtmlContent,CustomerId")] PDFModel pdfModel)
        {
            if (ModelState.IsValid)
            {
                db.PDFs.Add(pdfModel);
                db.SaveChanges();
                return RedirectToAction("Edit", new { Controller = "PDF", Id = pdfModel.Id });
            }

            return View(pdfModel);
        }

        // GET: PDF/PDFModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PDFModel pdfModel = db.PDFs.Find(id);
            if (pdfModel == null)
            {
                return HttpNotFound();
            }
            return View(pdfModel);
        }

        // POST: PDF/PDFModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,HtmlContent,CustomerId")] PDFModel pdfModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pdfModel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(pdfModel);
        }


        // POST: PDF/PDFModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        public ActionResult Generate(int id)
        {
            MemoryStream stream = new MemoryStream();
            PDFModel pdfModel = db.PDFs.Find(id);
            PDFService pdfService = new PDFService();
            PDFFileModel pdfFile = pdfService.HtmlToPdf("filename", pdfModel.HtmlContent, UserId, options: new string[] { "--disable-external-links", "--disable-internal-links" });
            
            return File(pdfFile.FilePathAndName, "application/pdf", pdfFile.FileName);
        }


        // GET: PDF/PDFModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PDFModel pdfModel = db.PDFs.Find(id);
            if (pdfModel == null)
            {
                return HttpNotFound();
            }
            return View(pdfModel);
        }

        // POST: PDF/PDFModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PDFModel pdfModel = db.PDFs.Find(id);
            db.PDFs.Remove(pdfModel);
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
