namespace TFOF.Areas.Core.Forms
{
    using System.Web.Mvc;
    using TFOF.Areas.Core.Models;
    using System.Collections.Generic;

    public class AppSearchForm : SearchForm
    {
        public AppSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("CoreAPI", "App", url);
            AddSearchField(new SearchField("Name", "Name", SearchField.Comparators.ContainsAny));
            AddSortField(new SortField("Name", "Name", true));
            Selects = new List<string>() { "Id", "Name", "Area", "TableName" };


        }
    }

    public class AppForm : Form
    {
        public AppForm()
        {
            FormTitle = "App";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "Area" });
            Fields.Add(new CharField() { Name = "Name" });
            Fields.Add(new CharField() { Name = "NamePlural" });
            Fields.Add(new CharField() { Name = "Title" });
            Fields.Add(new CharField() { Name = "TitlePlural" });
            Fields.Add(new CharField() { Name = "TableName" });
            Fields.Add(new BooleanField() { Name = "InheritsBaseModel" });

        }
    }

    public class AppFieldSearchForm :  SearchForm
    {
        public AppFieldSearchForm(UrlHelper url) : base(url)
        {
            //Construct SearchForms
            SetApiUrl("CoreAPI", "AppField", url);
            AddSearchField(new SearchField("Name", "Name", "Ex: Id", SearchField.Comparators.ContainsAny));
            AddSortField(new SortField("Name", "Name", true));
        }
    }
    public class AppFieldForm : Form
    {
        public AppFieldForm()
        {
            FormTitle = "App Field ";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new IdField() { Name = "AppId" });
            Fields.Add(new IntegerField() { Name = "Position" });
            Fields.Add(new CharField() { Name = "Name"});
            Fields.Add(new CharField() { Name = "DisplayName" });
            Fields.Add(new CharField() { Name = "ColumnName" });
            Fields.Add(new BooleanField() { Name = "IsPrimaryKey" });
            Fields.Add(new CharField() { Name = "ForeignKeyTo", HelpText ="Type the full path of the class. Example: ProjectName.Areas.Customer.Models.CustomerModel;"});
            Fields.Add(new CharField() { Name = "DataType", Options = AppFieldModel.dataTypes });
            Fields.Add(new IntegerField() { Name = "StringLength" });
            Fields.Add(new CharField() { Name = "FieldOptionRegistration" });
            Fields.Add(new BooleanField() { Name = "IsSearchable" });
            Fields.Add(new CharField() { Name = "DefaultValue" });
        }
    }
}