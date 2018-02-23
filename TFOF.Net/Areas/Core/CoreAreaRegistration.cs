namespace TFOF.Areas.Core
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System;

    public class CoreAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Core";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            try
            {
                context.Routes.MapHttpRoute(
                    this.AreaName + "API",
                    this.AreaName + "/{controller}/API/{id}/{method}",
                    new { name = this.AreaName, id = RouteParameter.Optional, method = RouteParameter.Optional }

                    );
            } catch(Exception e) {
                //do nothing
            }
            try{
                context.MapRoute(
                    this.AreaName + "Default",
                    this.AreaName + "/{controller}/{action}/{id}",
                    new { name = this.AreaName, action = "Index", id = UrlParameter.Optional }
                );
            }
            catch (Exception e)
            {
                //do nothing
            }

        }
    }
}