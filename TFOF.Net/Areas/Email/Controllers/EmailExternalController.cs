using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TFOF.Areas.Core.Controllers;
using TFOF.Areas.Email.Models;

namespace TFOF.Areas.Email.Controllers
{
    public class EmailExternalController : CoreController
	{
		private EmailModelContext db = new EmailModelContext();

		public ActionResult Index()
		{
			// to do -- Pagination and filters will be done as core lists requirement
			return View(db.EmailExternalLogs.ToList());
		}
	}
}