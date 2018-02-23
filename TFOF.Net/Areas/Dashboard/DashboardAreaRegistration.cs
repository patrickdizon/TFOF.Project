using System.Web.Mvc;
using TFOF.Areas.Core;

namespace TFOF.Areas.Dashboard
{
    public class DashboardAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Dashboard";
            }
        }

        //public override void RegisterArea(AreaRegistrationContext context) 
        //{
        //    context.MapRoute(
        //        "DashboardDefault",
        //        "Dashboard/{controller}/{action}/{id}",
        //        new { action = "Index", id = UrlParameter.Optional }
        //    );
        //}
    }
}