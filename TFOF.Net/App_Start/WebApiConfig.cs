using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TFOF
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            try{
                config.MapHttpAttributeRoutes();

            }
            catch (Exception e)
            {
                //do nothing
            }
            try{
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

            }
            catch (Exception e)
            {
                //do nothing
            }
            
        }
    }
}
