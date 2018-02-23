namespace TFOF.Areas.Contact.Controllers.API
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using Newtonsoft.Json.Linq;

    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.Contact.Models;

    public class ContactController : CoreController<ContactModel>
    {
        //PUT: Contact/Contact/API/5/method
        [ResponseType(typeof(void))]
        public override IHttpActionResult Put(string id, string method, JObject jsonObject)
        {
            ContactModel model = DB.Models.Find(id);

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