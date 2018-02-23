namespace TFOF.Areas.Report.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Web.Mvc;

    using TFOF.Areas.Core.Controllers;
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Report.Forms;
    using TFOF.Areas.Report.Models;
    using Services;
    using System.Collections.Generic;
    using Core.Models;
    using MvcReportViewer;
    using Core.Services;
    using System.Configuration;

    using Core.Attributes;
    using System.Linq;

    [SiteAuthorize]
    public class ReportController : CoreController<ReportModel>
    { 
        public ReportController()
        {
            Form = new ReportForm();
            TitlePlural = "Reports";
            SearchForms.Add(new ReportSearchForm(Url));
        }

        public ActionResult ChartTest()
        {
            return View();
        }

        public override ActionResult Index()
        {
            if (User.IsInRole(SiteRole.Administrators)) {
                return base.Index();
            }
            return Redirect(Url.Action("List", "Report"));
        }

        public ActionResult List()
        {
            List<ReportModel> reports = ReportService.GetReports(null, User);
            return View(reports);
        }

        [SiteAuthorize(Roles = SiteRole.Administrators)]
        public override ActionResult Edit(string id)
        {
            return base.Edit(id);
        }

        [SiteAuthorize(Roles = SiteRole.Administrators)]
        public override ActionResult Edit(ReportModel model)
        {
            return base.Edit(model);
        }
        
        // Post: Report/Report/Run/5
        public ActionResult Run(int id)
        {
            ReportModel reportModel = GetOne(id.ToString());
            if (reportModel != null)
            {
                if (ReportService.GetReport(reportModel, User) == null)
                {
                    return View("Error");
                }
            }
            ViewData["Form"] = ReportService.UserParameterForm(reportModel, Url);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Run()
        {
            int id;
            int.TryParse(Request.Params.Get("Id"), out id);
            if (id > 0)
            {
                ReportModel reportModel = GetOne(id.ToString());
                if (reportModel != null)
                {
                    if (ReportService.GetReport(reportModel, User) == null)
                    {
                        TempData["Error"] = "The report you are accessing does not exist, requires approrpiate access or is not active.";
                        return Redirect(Url.Action("Index", "Report", new { area = "Report" }));
                    }
                    reportModel.SetParameters(Request);
                    return ReportService.Run(reportModel, UserId).Download(Response, true, true);
                }
            }
            TempData["Error"] = "There was an error determining the Id or the report does not exist.";
            return Redirect(Url.Action("Run", "Report"));
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Viewer(int id)
        {
            ReportModel reportModel = GetOne(id.ToString());
            if (reportModel != null)
            {
                //ReportService.GetReport checks pe
                if(ReportService.GetReport(reportModel, User) == null)
                {
                    TempData["Error"] = "The report you are accessing does not exist, requires approrpiate access or is not active.";
                    return Redirect(Url.Action("Index", "Report", new { area = "Report" }));
                }
                ViewData["Breadcrumbs"] = new Breadcrumbs(Url)
                .AddCrumb("Reports", Url.Action("Index", "Report"))
                .AddCrumb("View");
                return View(ReportService.View(reportModel, UserId));
            }
            

            TempData["Error"] = "There was an error determining the Id or the report does not exist.";
            return Redirect(Url.Action("Index", "Report", new { area = "Report" }));
        }

    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class ReportParameterController : CoreController<ReportParameterModel>
    {
        public ReportParameterController()
        {
            CanDelete = true;
            ForeignKey = "ReportId";
            Form = new ReportParameterForm();
            ReturnTo = new ReturnTo(new ReportController(), "Edit", "ReportId", "Name", new string[] { "Create", "Edit", "Delete" });
            TitlePlural = "Reports";
        }

        public override ActionResult Index()
        {
            return Redirect(Url.Action("Index", "Report"));

        }

        public ActionResult List()
        {
            return base.Index();
        }

    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class ReportUserController : CoreController<ReportUserModel> {
        public ReportUserController()
        {
            CanDelete = true;
            CanCreate = false;
            ForeignKey = "ReportId";
            Form = new ReportUserForm();
            ReturnTo = new ReturnTo(new ReportController(), "Edit", "ReportId", "Name", new string[] { "Create", "Edit", "Delete" });
            TitlePlural = "Reports";
            SearchForms.Add(new ReportUserSearchForm(Url));
        }

        public override ActionResult Index()
        {
            return Redirect(Url.Action("Index", "Report"));
        }

        public ActionResult List()
        {
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Reports", Url.Action("Index", "Report"));
            breadcrumbs.AddCrumb("Report Users");
            ViewData["Breadcrumbs"] = breadcrumbs;
            return base.Index();
        }
    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class ReportRoleController : CoreController<ReportRoleModel>
    {
        public ReportRoleController()
        {
            CanDelete = true;
            ForeignKey = "ReportId";
            Form = new ReportRoleForm();
            ReturnTo = new ReturnTo(new ReportController(), "Edit", "ReportId", "Name", new string[] { "Create", "Edit", "Delete" });
            TitlePlural = "Reports";
        }

        public override ActionResult Index()
        {
            return Redirect(Url.Action("Index", "Report"));
        }
    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class ReportUserActivityLogController : CoreController<ReportUserActivityLogModel>
    {
        public ReportUserActivityLogController()
        {
            CanDelete = false;
            CanCreate = false;
            ForeignKey = "ReportId";
            TitlePlural = "Reports";
            SearchForms.Add(new ReportUserActivityLogSearchForm(Url));
        }

        public override ActionResult Index()
        {
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Reports", Url.Action("Index", "Report"));
            breadcrumbs.AddCrumb("User Activity Logs");
            ViewData["Breadcrumbs"] = breadcrumbs;
            return base.Index();
        }
    }

}