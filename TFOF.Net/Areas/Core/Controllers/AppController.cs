namespace TFOF.Areas.Core.Controllers
{
    using System.Web.Mvc;

    using TFOF.Areas.Core.Models;
    using TFOF.Areas.Core.Forms;
    using Services;
    using Attributes;

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class AppController : CoreController<AppModel>
    {
        public AppController()
        {
            Form = new AppForm();
            CanDelete = true;
            TitlePlural = "Apps";
            SearchForms.Add(new AppSearchForm(Url));
        }

        public ActionResult CreateArea(int id)
        {
            AppModel model = DB.Models.Find(id);
            bool success = new AppService().CreateApp(HttpContext, model);
            TempData["Success"] = "Successfully created app. The files are located in ~/App_Data/" + model.Area + ".";
            //return View(model);
            return Redirect(Url.Action("Edit", "App", new { id = id }));
        }
        
    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class AppFieldController : CoreController<AppFieldModel>
    {
        public AppFieldController()
        {
            DB = new AppFieldModelContext<AppFieldModel>();
            ForeignKey = "AppId";
            Form = new AppFieldForm();
            CanDelete = true;
            ReturnTo = new ReturnTo(new AppController(), "Edit", "AppId", "Name", new string[] { "Create", "Edit", "Delete" });
            TitlePlural = "App Fields";
            SearchForms.Add(new AppFieldSearchForm(Url));
        }

        public override ActionResult Create(AppFieldModel model)
        {
            var basereturn = base.Create(model);
            return Redirect(Url.Action("Create", "AppField", new { id = model.AppId }));
        }
    }
}
