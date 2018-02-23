using TFOF.Areas.Core;

namespace TFOF.Areas.Email
{
    public class EmailAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Email";
            }
        }
    }
}