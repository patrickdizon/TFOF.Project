using TFOF.Filters;
using Hangfire;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Configuration;
using System.Web.Hosting;

[assembly: OwinStartupAttribute(typeof(TFOF.Startup))]
namespace TFOF
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/Account/Account/Login"),

            });

            //if ((System.Environment.MachineName.ToUpper().Equals(ConfigurationManager.AppSettings["HangfireProductionWebServerName"].ToUpper()) && Globals.ISPRODUCTION) || !Globals.ISPRODUCTION)
            //{
            //    app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //    {
            //        Authorization = new[] { new HangfireAuthorizationFilter() }
            //    });
            //}
        }
    }
    
}
