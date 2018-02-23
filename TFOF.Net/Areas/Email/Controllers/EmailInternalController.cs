using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TFOF.Areas.Core.Controllers;
using TFOF.Areas.Email.Models;
using TFOF.Areas.Core.Forms;

namespace TFOF.Areas.Email.Controllers
{
    public class EmailInternalController : CoreController
	{
		private EmailModelContext db = new EmailModelContext();

		public ActionResult Index()
        {
			SearchForm searchForm = new SearchForm("EmailInternalControllerAPI", "EmailInternal", Url);
			searchForm.AddSearchField(new SearchField("Subject", "Subject", "Ex: Exception", SearchField.Comparators.ContainsAny));
			searchForm.AddSearchField(new SearchField("SentTo", "Sent To", "Ex: John", SearchField.Comparators.ContainsAny));
			searchForm.AddSearchField(new SearchField("Body", "Message Text", "Ex: Invoice", SearchField.Comparators.ContainsAny));

			searchForm.AddSortField(new SortField("Subject", "Subject", true));
			searchForm.AddSortField(new SortField("SentTo", "Sent To", true));
			searchForm.AddSortField(new SortField("SentDate", "Date",true));

			ViewData["SearchForm"] = searchForm;


			return View();
		}
    }
}