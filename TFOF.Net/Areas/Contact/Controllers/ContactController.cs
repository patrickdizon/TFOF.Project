namespace TFOF.Areas.Contact.Controllers
{
	using System.Collections.Generic;

	using TFOF.Areas.Core.Controllers;
	using TFOF.Areas.Contact.Forms;
	using TFOF.Areas.Contact.Models;
	using Core.Attributes;

	[SiteAuthorize(Roles = SiteRole.Contact)]
	public class ContactController : CoreController<ContactModel>
	{
		public ContactController()
		{
			CanCreate = true;
			//CanDelete = true;
			//ForeignKey = "";
			Form = new ContactForm();
			ReturnTo = new ReturnTo(this, "Index", null, null, new string[] { "Create", "Edit" });
			TitlePlural = "Contacts";
			SearchForms.Add(new ContactSearchForm(Url));
		}
		


	}
}
