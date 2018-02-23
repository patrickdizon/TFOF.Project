namespace TFOF.Areas.Home.Controllers
{
    using Core.Attributes;
    using Core.Controllers;
    using System.Web.Mvc;

    [SiteAuthorize]
    public class HomeController : CoreController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}