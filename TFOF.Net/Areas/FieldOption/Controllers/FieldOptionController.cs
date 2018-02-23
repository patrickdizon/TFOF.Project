namespace TFOF.Areas.FieldOption.Controllers
{
    using TFOF.Areas.FieldOption.Models;
    using TFOF.Areas.Core.Controllers;
    using TFOF.Areas.Core.Forms;
    using Forms;
    using System.Collections.Generic;
    using Core.Attributes;

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class FieldOptionController : CoreController<FieldOptionModel>
    {
        public FieldOptionController()
        {
            Form = new FieldOptionForm();
            TitlePlural = "Field Options";
            SearchForms.Add(new FieldOptionSearchForm(Url));
        }
    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class FieldOptionModelFieldController : CoreController<FieldOptionModelFieldModel> {
        public FieldOptionModelFieldController()
        {
            CanDelete = true;
            ForeignKey = "FieldOptionId";
            Form = new FieldOptionModelFieldForm();
            ReturnTo = new ReturnTo(new FieldOptionController(), "Edit", "FieldOptionId", "Name", new string[] { "Create", "Edit", "Delete" });
        }
    }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class FieldOptionValueController : CoreController<FieldOptionValueModel> {
        public FieldOptionValueController()
        {
            DB = new FieldOptionModelContext<FieldOptionValueModel>();
            CanDelete = true;
            ForeignKey = "FieldOptionId";
            Form = new FieldOptionValueForm();
            ReturnTo = new ReturnTo(new FieldOptionController(), "Edit", "FieldOptionId", "Name", new string[] { "Create", "Edit", "Delete" });
        }
    }
}
