using System.Web.Mvc;
using TFOF.Areas.Core;

namespace TFOF.Areas.Calendar
{
    public class CalendarAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Calendar";
            }
        }
    }
}