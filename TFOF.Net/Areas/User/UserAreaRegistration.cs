using TFOF.Areas.Core;

namespace TFOF.Areas.User
{
    public class UserAreaRegistration : CoreAreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "User";
            }
        } 
    }
}