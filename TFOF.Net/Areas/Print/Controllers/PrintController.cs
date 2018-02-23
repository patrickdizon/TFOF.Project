

namespace TFOF.Areas.Print.Controllers
{
    using System.Web.Mvc;
    using TFOF.Areas.Core.Controllers;
    using TFOF.Areas.Print.Forms;
    using TFOF.Areas.Print.Models;
    using TFOF.Areas.Print.Services;
    using Core.Models;
    using System;
    using Core.Services;

    public class PrintController : CoreController<PrintModel>
    {
        public PrintController()
        {
            CanCreate = false;
            CanDelete = true;
            Form = new PrintForm();
            TitlePlural = "Print";
            SearchForms.Add(new PrintSearchForm(Url));
        }
       
        //public ActionResult PrintDocument(int id)
        //{
        //    PrintService printService = new PrintService();
        //    //BackgroundJob.Enqueue(() => printService.processDocument(UserId,id));
        //    var serviceResult = printService.processDocument(UserId, id);
        //    if (serviceResult.HasErrors)
        //    {

        //        TempData["Error"] = serviceResult.Messages[0].Message;
        //    }
        //    else
        //    {
        //        TempData["Success"] = serviceResult.Messages[0].Message;
        //    }
        //    return Redirect(Url.Action("Index", "Print"));

        //}
    }

    public class PrintLogController : CoreController<PrintLogModel>
    {
        public PrintLogController()
        {
            CanCreate = false;
            CanDelete = true;
            TitlePlural = "Print";
            Form = new PrintLogForm();
            SearchForms.Add(new PrintLogSearchForm(Url));
        }

        public ActionResult Requeue(Int64 id)
        {
            BaseModelContext<PrintModel> printDb = new BaseModelContext<PrintModel>();
            PrintLogModel printLogModel = DB.Models.Find(id);

            if(printLogModel != null)
            {
                PrintModel printModel = new PrintModel()
                {
                    EntityName = printLogModel.EntityName,
                    PrimaryKey = printLogModel.PrimaryKey,
                    FileLocation = printLogModel.FileLocation,
                    PrinterId = printLogModel.PrinterId,
                    Status = PrintModel.queuedStatus,
                    RetryCount = 0,
                    DeleteFile = false,
                };
                printDb.Models.Add(printModel);
                printDb.SaveChanges();
                TempData["Success"] = "Successfully re-queued job id: " + printLogModel.PrintId.ToString() + ".";
            }

            return Redirect(Url.Action("Index", "Print"));

        }

        public override ActionResult Index()
        {
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Print Jobs", Url.Action("Index", "Print"));
            breadcrumbs.AddCrumb("Print Logs");
            ViewData["Breadcrumbs"] = breadcrumbs;
            return base.Index();
        }
    }

    public class PrinterController : CoreController<PrinterModel>
    {
        public PrinterController()
        {
            //CanCreate = false;
            //CanDelete = true;
            //ForeignKey = "";
            Form = new PrinterForm();
            TitlePlural = "Printers";
            SearchForms.Add(new PrinterSearchForm(Url));
        }

        public override ActionResult Edit(string id)
        {
            PrinterModel printerModel = GetOne(id);
            PrintService.GetPrinterTrays(printerModel);
            return base.Edit(id);
        }

    }
}