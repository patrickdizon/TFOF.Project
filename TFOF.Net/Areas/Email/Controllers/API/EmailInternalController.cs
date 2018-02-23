

namespace TFOF.Areas.Email.Controllers.API
{   
    using System.Web.Http.OData.Query;
    using System.Web.Http;

    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.Email.Models;
    using TFOF.Areas.Core.Forms;
    
    public class EmailInternalController : CoreController
    {
		private EmailModelContext db = new EmailModelContext();

		// GET: Email/EmailInternalLog/API
        public IHttpActionResult Get(ODataQueryOptions<EmailInternalLogModel> options)
        {
            return Ok(new PageResult<EmailInternalLogModel>(db.EmailInternalLogs, options));
        }
		

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	
	}
}
