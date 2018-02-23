using TFOF.Areas.Core.Services;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace TFOF.Areas.Core.Controllers
{
    [Authorize]
    public class HomeController : CoreController
    {   
        
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Selectize()
        {
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Welcome", Url.Action("Index", "Home"));
            breadcrumbs.AddCrumb("Selectize");
            ViewBag.Breadcrumbs = breadcrumbs;
            return View();
        }
       
    }

        

}
