

namespace TFOF.Areas.Print.Forms
{

    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Report.Models;
    using FieldOption.Models;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Printing;
    using System.Web.Mvc;
    using System.Security.Principal;

    public class PrintForm : Form
    {
        public PrintForm()
        {
            FormTitle = "Print";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "EntityName", Label = "Entity Name", HelpText = "The model name associated with the table." });
            Fields.Add(new CharField() { Name = "PrimaryKey", Label = "Primary Key", HelpText = "Primary Key of the model's record." });
            Fields.Add(new CharMaxField() { Name = "FileLocation", Label = "File Location", HelpText = "A file path, local or network." });
            Fields.Add(new CharField() { Name = "PrinterId", Label = "Printer", Options = PrintModelContext.GetPrinters()});
            Fields.Add(new IntegerField() { Name = "RetryCount", Label = "Retry Count", HelpText = "" });
            Fields.Add(new CharField() { Name = "Status", Label = "Status", HelpText = "", Options = PrintModel.Statuses });
            Fields.Add(new BooleanField() { Name = "DeleteFile", Label = "Delete File", HelpText = "Check to delete the file after it has been printed." });
            Fields.Add(new DateTimeField() { Name = "PrintedDateTime", Label = "Printed DateTime", HelpText = "The time of the day when it printed.", IsReadOnly = true });
            Fields.Add(new CharMaxField() { Name = "ErrorMessage", Label = "ErrorMessage", IsReadOnly = true });
        }
    }

    public class PrintSearchForm : SearchForm
    {
        public PrintSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("PrintAPI", "Print", url);
            AddSearchField(new SearchField("BatchId", "BatchId", SearchField.Comparators.Equal, SearchField.stringType));
            AddSearchField(new SearchField("PrinterId", "Printer", SearchField.Comparators.Equal, SearchField.intType, null, PrintModelContext.GetPrinters()));
            AddSearchField(new SearchField("EntityName", "EntityName", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("PrimaryKey", "PrimaryKey", SearchField.Comparators.StartsWith, SearchField.stringType));
            AddSearchField(new SearchField("PrintedDateTime", "Printed DateTime", SearchField.Comparators.Range, SearchField.datetimeType));
            AddSearchField(new SearchField("Status", "Status", SearchField.Comparators.Equal, SearchField.stringType, null, PrintModel.Statuses));
            AddSearchField(new SearchField("CreatedBy_LastName", "Printed by Last Name", SearchField.Comparators.StartsWith, SearchField.stringType, null));
            
            AddSortField(new SortField("PrinterName", "PrinterName"));
            AddSortField(new SortField("PrintedDateTime", "PrintedDateTime"));
            AddSortField(new SortField("Status", "Status"));
            Selects = new List<string>() {
                "Id",
                "EntityName",
                "PrimaryKey",
                "FileLocation",
                "Printer",
                "PrintedDateTime",
                "RetryCount",
                "Status",
                "ErrorMessage",
                "CreatedBy",
            };


            Expand = "Printer/PrinterTray,CreatedBy";
        }
    }
    public class PrintLogSearchForm : PrintSearchForm
    {
        public PrintLogSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("PrintAPI", "PrintLog", url);
            AddSortField(new SortField("PrintId", "Id", true, true));
            Selects.Add("PrintId");
            //Rest is inherited
        }
    }


    public class PrintLogForm : Form
    {
        public PrintLogForm()
        {   
            FormTitle = "Print Log";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "EntityName", Label = "Entity Name", HelpText = "The model name associated with the table." });
            Fields.Add(new CharField() { Name = "PrimaryKey", Label = "Primary Key", HelpText = "Primary Key of the model's record." });
            Fields.Add(new CharMaxField() { Name = "FileLocation", Label = "File Location", HelpText = "A file path, local or network." });
            Fields.Add(new CharField() { Name = "PrinterId", Label = "Printer Name", Options = PrintModelContext.GetPrinters() });
            Fields.Add(new IntegerField() { Name = "RetryCount", Label = "Retry Count", HelpText = "" });
            Fields.Add(new CharField() { Name = "Status", Label = "Status", HelpText = "", Options = PrintModel.Statuses });
            Fields.Add(new BooleanField() { Name = "DeleteFile", Label = "Delete File", HelpText = "Check to delete the file after it has been printed." });
            Fields.Add(new DateTimeField() { Name = "PrintedDateTime", Label = "Printed DateTime", HelpText = "The time of the day when it printed.", IsReadOnly = true });
            Fields.Add(new CharMaxField() { Name = "ErrorMessage", Label = "ErrorMessage", IsReadOnly = true });
        }
    }

    public class PrinterSearchForm : SearchForm
    {
        public PrinterSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("PrintAPI", "Printer", url);
            AddSearchField(new SearchField("Name", "", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("Description", "", SearchField.Comparators.ContainsAny));
            AddSearchField(new SearchField("Tray", "", SearchField.Comparators.ContainsAny));
            AddSortField(new SortField("Name", "", true));
            AddSortField(new SortField("Description", ""));

            Expand = "PrinterTray";

        }
    }


    public class PrinterForm : Form
    {
        public PrinterForm()
        {
            FormTitle = "Printer";
            Fields.Add(new IdField() { Name = "Id" });
            Fields.Add(new CharField() { Name = "Name", HelpText = "Local printer name or network path: \\\\hostname\\printer name" });
            Fields.Add(new CharField() { Name = "Description", HelpText = "User friendly description or Display Name." });
            Fields.Add(new CharField() { Name = "PrinterTrayId", Label="Tray", HelpText = "This list will populate after creation and if the printer is discovered."});
            Fields.Add(new BooleanField() { Name = "IsActive" });
        }

        public override Form Init(object model, IPrincipal user = null)
        {
            var init = base.Init(model, user);
            CharField printTrayIdField = (CharField)GetField("PrinterTrayId");
            IdField idField = (IdField)GetField("Id");
            printTrayIdField.Options = new SelectList(PrintModelContext.GetPrinterTrays(Convert.ToInt32(idField.Value)), "Value", "Text", "Group.Name", printTrayIdField.Value, null);;
            return init;
            
        }
    }
}
