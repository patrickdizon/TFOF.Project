using TFOF.Areas.Core.Forms;
using TFOF.Areas.FieldOption.Models;
using System.Web.Mvc;

namespace TFOF.Areas.FieldOption.Forms
{
    public class FieldOptionSearchForm : SearchForm
    {
        public FieldOptionSearchForm(UrlHelper url) : base(url)
        {
            
            SetApiUrl("FieldOptionAPI", "FieldOption", url);
            AddSearchField(new SearchField("Name", "Name", SearchField.Comparators.ContainsAny));
            AddSortField(new SortField("Name", "Name", true));
            Expand = "FieldOptionValues,FieldOptionModelFields";
        }
    }
    public class FieldOptionForm : Form
    {
        public FieldOptionForm()
        {
            FormTitle = "Field Option";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "Name" });
            Fields.Add(new CharField() { Name = "Slug" });
            
        }
    }
    public class FieldOptionValueForm : Form
    {
        public FieldOptionValueForm()
        {
            FormTitle = "Field Option Value";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new HiddenField() { Name = "FieldOptionId" });
            Fields.Add(new CharField() { Name = "Value" });
            Fields.Add(new IntegerField() { Name = "Position" });
            Fields.Add(new CharField() { Name = "Description" });
            Fields.Add(new BooleanField() { Name = "IsActive" });
            
        }
    }

    public class FieldOptionModelFieldForm : Form
    {
        public FieldOptionModelFieldForm()
        {
            FormTitle = "Field Option Model Field";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new HiddenField() { Name = "FieldOptionId" });
            Fields.Add(new CharField() { Name = "ModelField" });
            
        }
    }
}