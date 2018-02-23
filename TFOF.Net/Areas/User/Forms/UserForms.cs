namespace TFOF.Areas.User.Forms
{
    using TFOF.Areas.Core.Forms;
    using System.Web.Mvc;

    public class UserSearchForm : SearchForm
    {
        public UserSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("UserAPI", "User", url);
            AddSearchField(new SearchField("LastName", "Last Name", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("FirstName", "First Name", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("Email", "Email", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("LastLogin", "Last Login", SearchField.Comparators.Range));
            AddSearchField(new SearchField("TimeZone", "TimeZone", SearchField.Comparators.ContainsAny));

            AddSortField(new SortField("LastName", "Last Name", true));
            AddSortField(new SortField("FirstName", "First Name"));
            AddSortField(new SortField("Email", ""));
            AddSortField(new SortField("LastLogin", ""));
            AddSortField(new SortField("TimeZone", ""));


        }
    }

    public class CreateUserForm : Form
    {
        public CreateUserForm()
        {
            FormTitle = "User";
            Fields.Add(new CharField() { Name = "Email" });
            Fields.Add(new PasswordField() { Name = "Password" });
            Fields.Add(new CharField() { Name = "FirstName", ClassNames = "col-sm-6" });
            Fields.Add(new CharField() { Name = "LastName", ClassNames = "col-sm-6" });
            Fields.Add(new CharField() { Name = "Title", Value = "" });
            Fields.Add(new CharField() { Name = "TimeZone", Options = Account.Helpers.TimeZoneHelpers.GetTimeZones() });

        }
    }

    public class UserForm : Form
    {
        public UserForm()
        {
            FormTitle = "User";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "FirstName", ClassNames = "col-sm-6" });
            Fields.Add(new CharField() { Name = "LastName", ClassNames = "col-sm-6" });
            Fields.Add(new CharField() { Name = "Email" });
            Fields.Add(new CharField() { Name = "Title", Value = "" });
            Fields.Add(new CharField() { Name = "TimeZone", Options = Account.Helpers.TimeZoneHelpers.GetTimeZones() });
        }
    }

    public class RoleForm : Form
    {
        public RoleForm()
        {
            FormTitle = "Role";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "Name" });
        }
    }

    public class RoleSearchForm : SearchForm
    {
        public RoleSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("UserAPI", "Role", url);
            AddSearchField(new SearchField("Name", null, SearchField.Comparators.ContainsAny));
            AddSortField(new SortField("Name", null, true));
            AddSortField(new SortField("Created"));
            Expand = "UserRoles/User";
        }
    }

}