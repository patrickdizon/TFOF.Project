

namespace TFOF.Areas.File.Forms
{
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.File.Models;
    using System.Collections.Generic;
    using System.Web.Mvc;
    public class FileForm : Form
    {
      public FileForm()
        {
            FormTitle = "File";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "FileName", Label = "File Name", HelpText = "File name for export" });
            Fields.Add(new CharField() { Name = "FileLocation", Label = "File Location", HelpText = "Export file location." });
            Fields.Add(new CharField() { Name = "Action", Label = "Action", HelpText = "", Options = FileModel.Actions});
            Fields.Add(new CharField() { Name = "Status", Label = "Status", HelpText = "", Options = FileModel.Statues });

        }
    }


    public class FileSearchForm : SearchForm
    {
        public FileSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("FileAPI", "File", url);
            AddSearchField(new SearchField("Id", "Id", SearchField.Comparators.Equal, SearchField.stringType));
            AddSearchField(new SearchField("FileName", "FileName", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("FileLocation", "FileLocation", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("Action", "Action", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("Status", "Status", SearchField.Comparators.StartsWith, SearchField.stringType));

            AddSortField(new SortField("FileName", "FileName", true));
            AddSortField(new SortField("Action", "Action", true));
            AddSortField(new SortField("Status", "Status", true));
            Selects = new List<string>() {
                "Id",
                "FileName",
                "FileLocation",
                "Action",
                "Status"
            };
        }
    }

    public class FileLogSearchForm : SearchForm
    {
        public FileLogSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("ReportAPI", "FileLog", url);
            AddSearchField(new SearchField("User__FirstName", "First Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("User__LastName", "Last Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("File__Name", "File Name", SearchField.Comparators.StartsWith, SearchField.stringType));

            AddSortField(new SortField("File__Name", "File Name"));
            AddSortField(new SortField("User__LastName", "Last Name"));
            AddSortField(new SortField("User__FirstName", "First Name"));
            AddSortField(new SortField("Created", "Date", true, true));
            Selects = new List<string>() {
                "Id",
                "FileId",
                "UserId",
                "Action",
                "Created",
                "User",
                "File"
            };
            //Expand = "Report,User";
        }
    }

    public class FileDirectorySearchForm : SearchForm
    {
        public FileDirectorySearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("FileAPI", "FileDirectory", url);
            AddSearchField(new SearchField("Name", "", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("Path", "", SearchField.Comparators.ContainsAny));
            AddSortField(new SortField("Name", "", true));
            AddSortField(new SortField("Path", ""));

        }
    }


    public class FileDirectoryForm : Form
    {
        public FileDirectoryForm()
        {
            FormTitle = "File Directory";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "Name" });
            Fields.Add(new CharMaxField() { Name = "Path", Rows = 4, HelpText = "Dont forget the ending slash!"});


        }
    }
}