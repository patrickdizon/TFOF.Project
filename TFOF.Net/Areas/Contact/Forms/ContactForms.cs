namespace TFOF.Areas.Contact.Forms
{
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Contact.Models;
    using System.Web.Mvc;

	public class ContactSearchForm : SearchForm
    {
        public ContactSearchForm(UrlHelper url) : base(url)
        {
            SetApiUrl("ContactAPI", "Contact", url);
			AddSearchField(new SearchField("CompanyName", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("Category", "", SearchField.Comparators.Equal, SearchField.stringType, null, ContactModel.categories));
			AddSearchField(new SearchField("MainPhoneNumber", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("Address", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("City", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("State", "", SearchField.Comparators.Equal, SearchField.stringType, null, ContactModel.states));
			AddSearchField(new SearchField("ZipCode", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("AccountNumber", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("FirstName", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("LastName", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("PhoneNumber", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("MobileNumber", "", SearchField.Comparators.ContainsAny));
			AddSearchField(new SearchField("Email", "", SearchField.Comparators.StartsWith));
			AddSortField(new SortField("CompanyName", "", true));
			AddSortField(new SortField("Category", ""));
			AddSortField(new SortField("MainPhoneNumber", ""));
			AddSortField(new SortField("Address", ""));
			AddSortField(new SortField("City", ""));
			AddSortField(new SortField("State", ""));
			AddSortField(new SortField("ZipCode", ""));
			AddSortField(new SortField("AccountNumber", ""));
			AddSortField(new SortField("Region", ""));
			AddSortField(new SortField("FirstName", ""));
			AddSortField(new SortField("LastName", ""));
			AddSortField(new SortField("PhoneNumber", ""));
			AddSortField(new SortField("MobileNumber", ""));
			AddSortField(new SortField("Email", ""));
			
        }
    }


    public class ContactForm : Form
    {
        public ContactForm()
        {
            FormTitle = "Contact";
			Fields.Add(new IdField() { Name = "Id" });
			Fields.Add(new CharField() { Name = "CompanyName" });
			Fields.Add(new CharField() { Name = "Category", Options = ContactModel.categories });
			Fields.Add(new CharField() { Name = "MainPhoneNumber", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "MainFaxNumber", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "Address" });
			Fields.Add(new CharField() { Name = "City", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "State", ClassNames = "col-sm-3", Options = ContactModel.states });
			Fields.Add(new CharField() { Name = "ZipCode", ClassNames = "col-sm-3" });
			Fields.Add(new CharField() { Name = "Website" });
			Fields.Add(new CharField() { Name = "AccountNumber", ClassNames = "col-sm-6" });
            Fields.Add(new CharMaxField() { Name = "Notes", Rows = 3 });

            Fields.Add(new GroupLabel() { Label = "Staff" });
            Fields.Add(new CharField() { Name = "FirstName", ClassNames = "col-sm-6"});
			Fields.Add(new CharField() { Name = "LastName", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "PhoneNumber",  ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "PhoneNumberExtension", Label = "Extension", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "MobileNumber", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "AlternativePhoneNumber", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "FaxNumber", ClassNames = "col-sm-6" });
            Fields.Add(new CharField() { Name = "Email", ClassNames = "col-sm-6" });
            Fields.Add(new GroupLabel() { Label = "Billing Address" });

            Fields.Add(new CharField() { Name = "BillingAddress", Label = "Address" });
			Fields.Add(new CharField() { Name = "BillingCity", Label = "City", ClassNames = "col-sm-6" });
			Fields.Add(new CharField() { Name = "BillingState", Label = "State", ClassNames = "col-sm-3", Options = ContactModel.states });
			Fields.Add(new CharField() { Name = "BillingZipCode", Label = "Zip Code", ClassNames = "col-sm-3" });
			

			
        }
    }
}