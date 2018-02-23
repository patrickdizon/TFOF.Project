namespace TFOF.Areas.Core.Forms
{
    using System.Web.Mvc;
    using TFOF.Areas.Core.Models;

    public class AuditSearchForm : SearchForm
    {
        public AuditSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("CoreAPI", "Audit", url);
            AddSearchField(new SearchField("EntityName", "Entity Name", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("PrimaryKey", "Primary Key", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("PropertyName", "Property Name", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("CreatedBy__LastName", "Last Name", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("CreatedBy__FirstName", "First Name", SearchField.Comparators.ContainsAny));
            AddSortField(new SortField("Id", "Id", true, true));
            AddSortField(new SortField("Created", "Created", true, true));
            Expand = "CreatedBy";
        }
    }
    public class AuditViewerSearchForm : SearchForm
    {
            public AuditViewerSearchForm(UrlHelper url, string entityName, string primaryKey) : base(url)
        {
            SetApiUrl("CoreAPI", "Audit", url);
            AddSearchField(new SearchField("PropertyName", "Field", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("CreatedBy__LastName", "Last Name", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("CreatedBy__FirstName", "First Name", SearchField.Comparators.ContainsAny));
            RestrictSearchFields.Add(new RestrictSearchField("EntityName", new string[] { entityName }, RestrictSearchField.Comparators.Equal, RestrictSearchField.stringType));
            RestrictSearchFields.Add(new RestrictSearchField("PrimaryKey", new string[] { primaryKey }, RestrictSearchField.Comparators.Equal, RestrictSearchField.stringType));
            AddSortField(new SortField("Created", "Created", true, true));
            UseLocalStorage = false;
            Load = false;
            Expand = "CreatedBy";
        }
    }
}