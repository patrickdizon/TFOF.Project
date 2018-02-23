namespace TFOF.Areas.Calendar.Forms
{
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Calendar.Models;
    using System.Web.Mvc;

	public class CalendarSearchForm : SearchForm
    {
        public CalendarSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("CalendarAPI", "Calendar", url);
			AddSearchField(new SearchField("Date", "", SearchField.Comparators.Range, SearchField.datetimeType));
			AddSearchField(new SearchField("Name", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("IsHoliday", "", SearchField.Comparators.Equal,SearchField.boolType));
			AddSearchField(new SearchField("IsClosedForBusiness", "", SearchField.Comparators.Equal, SearchField.boolType));
			AddSortField(new SortField("Date", "", true));
			AddSortField(new SortField("Name", ""));
			AddSortField(new SortField("IsHoliday", ""));
			AddSortField(new SortField("IsClosedForBusiness", ""));
			
        }
    }


    public class CalendarForm : Form
    {
        public CalendarForm()
        {
            FormTitle = "Calendar";
			Fields.Add(new IdField() { Name = "Id" });
			Fields.Add(new DateTimeField() { Name = "Date" });
			Fields.Add(new CharField() { Name = "Name" });
			Fields.Add(new BooleanField() { Name = "IsHoliday" });
			Fields.Add(new CharField() { Name = "Notes" });
			Fields.Add(new BooleanField() { Name = "IsClosedForBusiness" });

			
        }
    }
}