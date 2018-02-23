using TFOF.Areas.Core;

namespace TFOF.Areas.Home
{
    public class HomeAreaRegistration : CoreAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Home";
            }
        }
       
    }
}