namespace TFOF.Areas.Core.Controllers.API
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using Newtonsoft.Json.Linq;

    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.Core.Models;

    public class ParadoxTableController : CoreController<ParadoxTableModel>
    {
        //PUT: ParadoxTable/ParadoxTable/API/5/method
        [ResponseType(typeof(void))]
        public override IHttpActionResult Put(string id, string method, JObject jsonObject)
        {
            ParadoxTableModel model = DB.Models.Find(id);

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