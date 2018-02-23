namespace TFOF.Areas.Core.Forms
{
    using TFOF.Areas.Core.Models;
    using System.Web.Mvc;

    public class ParadoxTableSearchForm : SearchForm
	{
		public ParadoxTableSearchForm(UrlHelper url) : base(url)
		{
			SetApiUrl("CoreAPI", "ParadoxTable", url);
			AddSearchField(new SearchField("Table", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("Sync", "", SearchField.Comparators.Equal, SearchField.boolType, null, SearchField.yesNoOptions));
			AddSearchField(new SearchField("MaxThreads", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("SyncInterval", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("SyncOnChange", "", SearchField.Comparators.Equal, SearchField.boolType, null, SearchField.yesNoOptions));
			AddSearchField(new SearchField("PostTableCreateScript", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("LastSync", "", SearchField.Comparators.Range));
			AddSearchField(new SearchField("LastSyncDuration", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("TableCorrupt", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("LastError", "", SearchField.Comparators.ContainsAny));
			AddSortField(new SortField("Table", ""));
			AddSortField(new SortField("Sync", "", true));
			AddSortField(new SortField("MaxThreads", ""));
			AddSortField(new SortField("SyncInterval", ""));
			AddSortField(new SortField("SyncOnChange", ""));
			AddSortField(new SortField("PostTableCreateScript", ""));
			AddSortField(new SortField("LastSync", ""));
			AddSortField(new SortField("LastSyncDuration", ""));
			AddSortField(new SortField("TableCorrupt", ""));
			AddSortField(new SortField("LastError", ""));
			
		}
	}


	public class ParadoxTableForm : Form
	{
		public ParadoxTableForm()
		{
			FormTitle = "Paradox Table";
			Fields.Add(new IdField() { Name = "Id" });
			Fields.Add(new ReadOnlyField() { Name = "Table" });
			Fields.Add(new BooleanField() { Name = "Sync" });
			Fields.Add(new IntegerField() { Name = "MaxThreads" });
			Fields.Add(new CharField() { Name = "SyncInterval", Options = ParadoxTableModel.syntIntervals });
			Fields.Add(new BooleanField() { Name = "SyncOnChange" });
			Fields.Add(new ReadOnlyField() { Name = "PostTableCreateScript" });
			Fields.Add(new ReadOnlyField() { Name = "LastSync" });
			Fields.Add(new ReadOnlyField() { Name = "LastSyncDuration" });
			Fields.Add(new ReadOnlyField() { Name = "TableCorrupt" });
			Fields.Add(new ReadOnlyField() { Name = "LastError" });





		}
	}
}