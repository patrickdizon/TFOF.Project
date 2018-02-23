using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web;
using System.Configuration;
using Hangfire;
using Microsoft.SqlServer.Types;
using System.Data.Entity.SqlServer;

namespace TFOF
{
    public class MvcApplication : System.Web.HttpApplication
	{
		private BackgroundJobServer _backgroundJobServer;

        protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			//RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			System.Web.Http.GlobalConfiguration.Configuration.Formatters.Remove(System.Web.Http.GlobalConfiguration.Configuration.Formatters.XmlFormatter);
			
			Globals.ISPRODUCTION = (ConfigurationManager.AppSettings["buildEnvironment"].ToString() == "PROD") ? true : false;
			Globals.ENVIRONMENT = ConfigurationManager.AppSettings["buildEnvironment"].ToString();
			Globals.DB = ConfigurationManager.ConnectionStrings["BaseModelContext"].ConnectionString.Split(';')[0].ToString();
           

            /// <remarks>
            ///Patrick Dizon 11/1/2016
            /// </remarks>
            /// 

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            //if ((System.Environment.MachineName.ToUpper().Equals(ConfigurationManager.AppSettings["HangfireProductionWebServerName"].ToUpper()) && Globals.ISPRODUCTION) || !Globals.ISPRODUCTION)
            //{
            //    Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireConnection");//.UseMsmqQueues(@".\hangfire-{0}");
            //    _backgroundJobServer = new BackgroundJobServer();
            //}

            // Enables use of spatial data types
            //SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));

            SqlProviderServices.SqlServerTypesAssemblyName = typeof(SqlGeography).Assembly.FullName;

        }

		protected void Application_BeginRequest(Object source, EventArgs e)
		{
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;

            if (!context.Request.IsLocal)
            {
                if (!Context.Request.IsSecureConnection
                    && Globals.ISPRODUCTION // to avoid switching to https when local testing
                    )
                {
                    // Only insert an "s" to the "http:", and avoid modifying http: in the url parameters
                    Response.Redirect(Context.Request.Url.ToString().Insert(4, "s"));
                }
            }
		}

		protected void Application_End()
		{
			_backgroundJobServer.Dispose();
		}

      
    }
}
