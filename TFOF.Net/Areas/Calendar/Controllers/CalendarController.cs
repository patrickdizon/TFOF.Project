namespace TFOF.Areas.Calendar.Controllers
{
	using System.Collections.Generic;

	using TFOF.Areas.Core.Controllers;
	using TFOF.Areas.Core.Forms;
	using TFOF.Areas.Calendar.Forms;
	using TFOF.Areas.Calendar.Models;

	public class CalendarController : CoreController<CalendarModel>
	{
		public CalendarController()
		{
			//CanCreate = false;
			//CanDelete = true;
			//ForeignKey = "";
			Form = new CalendarForm();
			ReturnTo = new ReturnTo(this, "Index", null, "Name", new string[] { "Create", "Edit" });
			TitlePlural = "Calendar";
			SearchForms.Add(new CalendarSearchForm(Url));
		}

	}
}
