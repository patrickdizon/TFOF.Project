namespace TFOF.Areas.Core.Controllers
{
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Core.Models;
    using Attributes;

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class ParadoxTableController : CoreController<ParadoxTableModel>
    {
        public ParadoxTableController()
        {
			CanCreate = false;
			Form = new ParadoxTableForm();
			TitlePlural = "Paradox Table";
			SearchForms.Add(new ParadoxTableSearchForm(Url));
        }

    }
}
