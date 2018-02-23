namespace TFOF.Areas.Calendar.Controllers.API
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using Newtonsoft.Json.Linq;

    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.Calendar.Models;

    public class CalendarController : CoreController<CalendarModel>
    {
        //PUT: Calendar/Calendar/API/5/method
        [ResponseType(typeof(void))]
        public override IHttpActionResult Put(string id, string method, JObject jsonObject)
        {
            CalendarModel model = DB.Models.Find(id);

            if (model != null)
            {
                if (method.Equals(""))
                {
					//Do something
				}
            }
                        
            return Ok(model);
        }
    }
}