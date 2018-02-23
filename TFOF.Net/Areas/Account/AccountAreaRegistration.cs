using System.Web.Mvc;
using TFOF.Areas.Core;
namespace TFOF.Areas.Account
{
    public class AccountAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Account";
            }
        }


    }
}