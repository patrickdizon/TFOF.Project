using System.Web.Mvc;
using TFOF.Areas.Core;

namespace TFOF.Areas.Print
{
    public class PrintAreaRegistration : CoreAreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Print";
            }
        }
    }
}