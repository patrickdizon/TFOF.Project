namespace TFOF.Areas.Account.Services
{
    using TFOF.Areas.Account.Models;
    using TFOF.Areas.Core.Helpers;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class AccountService
    {

        public static async System.Threading.Tasks.Task<bool> ResetPassword(
            ApplicationUser user, 
            ControllerContext controllerContext,
            HttpRequestBase request, 
            ApplicationUserManager userManager, 
            UrlHelper url
        )
        {
            // Send an email with this link
            Dictionary<string, string> emailContent = new Dictionary<string, string>()
                {
                    { "CallBackUrl", url.Action(
                        "ResetPassword",
                        "Account",
                        new { userId = user.Id, area = "Account", code = await userManager.GeneratePasswordResetTokenAsync(user.Id) },
                        protocol: request.Url.Scheme)
                    },
                    { "FirstName", user.FirstName },
                    { "ContactUs", ConfigurationManager.AppSettings.Get("AdminEmail") }
                };
            string mailBody = RazorHelper.Render(emailContent, controllerContext, "~/Areas/Account/Templates/ResetPasswordEmailTemplate.cshtml");
            await userManager.SendEmailAsync(user.Id, "Forgot Password", mailBody);

            return true;
        }
    }
}