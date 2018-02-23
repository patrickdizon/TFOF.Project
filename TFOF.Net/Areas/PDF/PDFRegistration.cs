using TFOF.Areas.Core;

namespace TFOF.Areas.PDF
{
    public class PDFAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PDF";
            }
        }
    }
}