namespace TFOF.Areas.Report.Forms
{
    using Core.Models;
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Report.Models;
    using FieldOption.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using User.Models;

    public class ReportForm : Form
    {
        public ReportForm()
        {
            FormTitle = "Report";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "Name" });
            Fields.Add(new CharField() { Name = "SSRSReportName", Label = "SSRS Report Name", HelpText="File name of the report in the SSRS server. Example: Report.rdl." });
            Fields.Add(new CharField() { Name = "Category", Label = "Category/Area", Options = FieldOptionModelContext.GetByRegistration("ReportModel.Category") });
            Fields.Add(new CharMaxField() { Name = "Description" });
            Fields.Add(new CharField() { Name = "Permit", Options = ReportModel.Permits, ClassNames = "col-sm-6"});
            Fields.Add(new BooleanField() { Name = "IsActive", ClassNames = "col-sm-6" });

            Fields.Add(new GroupLabel() { Label = "Run file name" });
            Fields.Add(new CharField() { Name = "FileName", ClassNames = "col-sm-9", HelpText = "To append date and/or time to end of file use {date} or {datetime}. Example: file-{date}" });
            Fields.Add(new CharField() { Name = "Format", ClassNames = "col-sm-3", Options = ReportModel.Formats });
            Fields.Add(new HiddenField() { Name = "ScheduledMonth", Label = "Months", HelpText = "Comma separated values of months from 0-11." });
            Fields.Add(new HiddenField() { Name = "ScheduledWeek", Label = "Weeks", HelpText = "Comma separated values of weeks in a month from 0-4." });
            Fields.Add(new HiddenField() { Name = "ScheduledDays", Label = "Days", HelpText = "Comma separated values of days in a week from 0-7." });
            Fields.Add(new HiddenField() { Name = "ScheduledTime", Label = "Time", HelpText = "The time of the day when it should run." });
            Fields.Add(new HiddenField() { Name = "LastRan" });
            
        }
    }


    public class ReportSearchForm : SearchForm
    {
        public ReportSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("ReportAPI", "Report", url);
            AddSearchField(new SearchField("Name", "Display Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("SSRSReportName", "Report File Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("Category", "Category", SearchField.Comparators.Equal, SearchField.stringType, null, ReportModel.Categories));
            AddSearchField(new SearchField("IsActive", "Active", SearchField.Comparators.Equal, SearchField.boolType, null, SearchField.yesNoOptions));
            AddSortField(new SortField("Name", "Name", true));
            AddSortField(new SortField("Category", "Category", true));
            Selects = new List<string>() {
                "Id",
                "Name",
                "Description",
                "Permit",
                "Format",
                "Category",
                "LastRun",
                "ScheduledMonth",
                "ScheduledWeek",
                "ScheduledDays",
                "ScheduledTime",
                "IsActive",
                "ReportParameters",
                "ReportUsers",
                "ReportRoles"
            };
            Expand = "ReportParameters,ReportUsers,ReportRoles";
        }
    }

    public class ReportParameterForm : Form
    {

        public ReportParameterForm()
        {
            FormTitle = "Report Parameter";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new IdField() { Name = "ReportId" });
            Fields.Add(new CharField() { Name = "Name", HelpText = "A variable type value" });
            Fields.Add(new CharField() { Name = "Label", HelpText = "Human friendly text" });
            Fields.Add(new CharField() { Name = "DataType", Options = ReportParameterModel.dataTypes, HelpText = "Determines the input field." });
            Fields.Add(new CharMaxField() { Name = "DefaultValue", HelpText = "A value used as a default" });
            Fields.Add(new CharMaxField() { Name = "FieldOptionSlug", HelpText = "In addition to Field Option slugs are Region and " });
            Fields.Add(new CharMaxField() { Name = "Description", HelpText = "Describe your parameter here to appear as a help text in run forms." });

        }
    }


    public class ReportUserForm : Form
    {

        public ReportUserForm()
        {
            FormTitle = "Report User";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new IdField() { Name = "ReportId" });
            Fields.Add(
                new CharField() {
                    Name = "UserId",
                    Label = "User",
                    Widget = new SelectizeWidget()
                    {
                        ApiUrl = GetUrl("UserAPI", "User"),
                        Filters = "FirstName,LastName,Email",
                        Label = "FirstName,LastName",
                        ValueField = "Id",
                        SearchType = SearchField.Comparators.StartsWith,
                        LabelFields = "FirstName,LastName,-Id,-Email",
                        OrderBy = "FirstName,LastName",
                        Top = 100
                        
                    },
                    Placeholder = "Search user by first, last name or email."

                }
            );
            Fields.Add(new BooleanField() { Name = "SendEmail", HelpText = "Check to send reports by email." , ClassNames = "col-sm-6" });
            Fields.Add(new BooleanField() { Name = "SendNotification",  HelpText = "Check to send notification of report availability.", ClassNames = "col-sm-6" });

        }
    }

    public class ReportUserSearchForm : SearchForm
    {
        public ReportUserSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("ReportAPI", "ReportUser", url);
            AddSearchField(new SearchField("User__FirstName", "First Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("User__LastName", "Last Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("Report__Name", "Report Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSortField(new SortField("Report__Name", "Report Name", true));
            AddSortField(new SortField("User__LastName", "Last Name", true));
            AddSortField(new SortField("User__FirstName", "First Name", true));
            AddSortField(new SortField("Created", "Date"));
            Selects = new List<string>() {
                "Id",
                "ReportId",
                "UserId",
                "SendEmail",
                "SendNotification",
                "Created",
                "User",
                "Report"
            };
            Expand = "Report,User";
        }
    }


    public class ReportRoleForm : Form
    {

        public ReportRoleForm()
        {
            FormTitle = "Report Role";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new IdField() { Name = "ReportId" });

            SelectList roleSelectList = new SelectList(new BaseModelContext<RoleModel>().Models.OrderBy(o => o.Name).ToList(), "Id", "Name");
            Fields.Add(new CharField() { Name = "RoleId", Label = "Role", Options = roleSelectList });
           
        }
    }


    public class ReportUserActivityLogSearchForm : SearchForm
    {
        public ReportUserActivityLogSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("ReportAPI", "ReportUserActivityLog", url);
            AddSearchField(new SearchField("User__FirstName", "First Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("User__LastName", "Last Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("Report__Name", "Report Name", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSortField(new SortField("Report__Name", "Report Name"));
            AddSortField(new SortField("User__LastName", "Last Name"));
            AddSortField(new SortField("User__FirstName", "First Name"));
            AddSortField(new SortField("Created", "Date", true, true));
            Selects = new List<string>() {
                "Id",
                "ReportId",
                "UserId",
                "Activity",
                "Created",
                "User",
                "Report"
            };
            Expand = "Report,User";
        }
    }

}