namespace TFOF.Areas.Core.Controllers
{
    using TFOF.Areas.Core.Attributes;
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Core.Services;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;

    [SiteAuthorize]
    public class ErrorController : Controller
    {
        public ErrorController()
        {
           
        }

        public static Dictionary<string, string> Layouts = new Dictionary<string, string>()
        {
            { "Modal" , "~/Views/Shared/_ModalLayout.cshtml" },
            { "ModalLarge" , "~/Views/Shared/_ModalLargeLayout.cshtml" }
        };

        public ActionResult Index() {

            List<string> roles = Request.QueryString.Get("Roles").Split(',').ToList();
            return View(roles);
        }

    }
}
