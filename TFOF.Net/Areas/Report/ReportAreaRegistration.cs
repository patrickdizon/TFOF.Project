using TFOF.Areas.Core;

namespace TFOF.Areas.Report
{
    public class ReportAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Report";
            }
        }
    }
}