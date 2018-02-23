using System.Web.Mvc;

using TFOF.Areas.Core.Models;
using System.Linq;
using TFOF.Areas.User.Models;

namespace TFOF.Areas.Core.Forms
{
    public class MenuForm : Form
    {
        public MenuForm()
        {
            FormTitle = "Menu";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "ParentId", Label = "Parent Menu", Options = new SelectList(new BaseModelContext<MenuModel>().Models.Where(w => w.ParentId == null).OrderBy(o => o.Label), "Id", "Label") });
            Fields.Add(new IntegerField() { Name = "GroupNumber" });
            Fields.Add(new IntegerField() { Name = "Position" });
            Fields.Add(new CharField() { Name = "Label" });
            Fields.Add(new CharField() { Name = "Icon", Placeholder = "A font awesome class name" });
            Fields.Add(new CharField() { Name = "Role", Options = new SelectList(new BaseModelContext<RoleModel>().Models.OrderBy(o => o.Name), "Name","Name") });
            Fields.Add(new CharField() { Name = "Action" });
            Fields.Add(new CharField() { Name = "Area" });
            Fields.Add(new CharField() { Name = "Controller" });
            Fields.Add(new CharField() { Name = "Environment", Options = MenuModel.environments });
        }
    }

    public class MenuSearchForm: SearchForm
    {
        public MenuSearchForm(UrlHelper url): base(url)
        {
            SetApiUrl("CoreAPI", "Menu", url);
            AddSearchField(new SearchField("Label", "Label", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("Role", "Role", SearchField.Comparators.Equal, SearchField.stringType, null, new SelectList(new BaseModelContext<RoleModel>().Models.OrderBy(o => o.Name), "Name", "Name")));
            AddSearchField(new SearchField("Environment", "Environment", SearchField.Comparators.Equal, SearchField.stringType, null, MenuModel.environments));
            AddSortField(new SortField("Parent__Label", " Parent", true));
            AddSortField(new SortField("Role", "Role"));
            AddSortField(new SortField("GroupNumber", " Group", true));
            AddSortField(new SortField("Environment", " Environment"));
            AddSortField(new SortField("Position", "", true));
            Expand = "Parent";
        }
    }
}