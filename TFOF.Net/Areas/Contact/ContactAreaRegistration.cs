using System.Web.Mvc;
using TFOF.Areas.Core;

namespace TFOF.Areas.Contact
{
    public class ContactAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Contact";
            }
        }
    }
}