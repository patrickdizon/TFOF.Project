

namespace TFOF.Areas.File.Controllers
{
    using System.Web.Mvc;
    using TFOF.Areas.Core.Controllers;
    using TFOF.Areas.File.Forms;
    using TFOF.Areas.File.Models;
    using TFOF.Areas.File.Services;
    using System;
    using Core.Attributes;
    using Core.Services;

    public class FileController : CoreController<TFOF.Areas.File.Models.FileModel>
    {
        public FileController()
        {
            CanCreate = false;
            CanDelete = true;
            Form = new FileForm();
            TitlePlural = "Files";
            SearchForms.Add(new FileSearchForm(Url));
        }



        public ActionResult ProcessFileAction(Int64? Id)
        {
            FileService fileService = new FileService();
            var serviceReturn=fileService.ProcessFileAction(UserId, Url.Action("Index", "File", null, Request.Url.Scheme),Id);
            if (serviceReturn != null && serviceReturn.Messages.Count > 0)
            {
                TempData["Success"] = serviceReturn.Messages;
            }
            return Redirect(Url.Action("Index", "File"));
        }

        [SiteAuthorize(Roles = SiteRole.Administrators)]
        public override ActionResult Edit(string id)
        {
            return base.Edit(id);
        }

        [SiteAuthorize(Roles = SiteRole.Administrators)]
        public override ActionResult Edit(TFOF.Areas.File.Models.FileModel model)
        {
            return base.Edit(model);
        }

    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class FileLogController : CoreController<FileLogModel>
    {
        public FileLogController()
        {
            CanDelete = false;
            CanCreate = false;
            ForeignKey = "ReportId";
            TitlePlural = "Reports";
            SearchForms.Add(new FileLogSearchForm(Url));
        }

        public override ActionResult Index()
        {
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("File", Url.Action("Index", "File"));
            breadcrumbs.AddCrumb("User Activity Logs");
            ViewData["Breadcrumbs"] = breadcrumbs;
            return base.Index();
        }
    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class FileDirectoryController : CoreController<FileDirectoryModel>
    {
        public FileDirectoryController()
        {
            //CanCreate = false;
            //CanDelete = true;
            //ForeignKey = "";
            Form = new FileDirectoryForm();
            ReturnTo = new ReturnTo(this, "Index", null, "Name", new string[] { "Create", "Edit" });
            TitlePlural = "File Directory";
            SearchForms.Add(new FileDirectorySearchForm(Url));
        }

    }
}