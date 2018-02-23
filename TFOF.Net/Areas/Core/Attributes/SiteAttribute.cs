
namespace TFOF.Areas.Core.Attributes
{
	using System.Web.Http.Controllers;
	using System.Web.Mvc;
	using System.Web.Routing;
	using System.Net.Http;

	/// <summary>
	/// Use SiteAuthorize instead of Authorize attribute to provide a customized error page for 404s.
	/// Optionally, use this with SiteRoles. 
	/// </summary>
	public class SiteAuthorize : System.Web.Mvc.AuthorizeAttribute
	{
	  
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				if (!filterContext.HttpContext.User.IsInRole(SiteRole.Administrators))
				{
					filterContext.Result = new RedirectToRouteResult(new
						RouteValueDictionary(new { controller = "Error", area = "Core", Roles = string.Join(",", Roles) }));
				}
			}
			else
			{
				filterContext.Result = new HttpUnauthorizedResult();
			}

		}
	}

	/// <summary>
	/// Use APIAuthorize instead of Authorize attribute to provide a customized error page for 404s.
	/// Optionally, use this with SiteRoles. 
	/// </summary>
	public class APIAuthorize : System.Web.Http.AuthorizeAttribute
	{

		protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
		{
			if (filterContext.RequestContext.Principal.Identity.IsAuthenticated)
			{
				if (!filterContext.RequestContext.Principal.IsInRole(SiteRole.Administrators))
				{
					filterContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
				}
			}
			else
			{
				filterContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
			}
		}
	}
	/// <summary>
	/// SiteRole helper. Each const value must be present in the AspNetRoles Table.
	/// You will use these constants along with SiteAuthorize to set the authorization level on the controller.
	/// Example: [SiteAuthorize(Roles = SiteRole.Administrators)]
	/// </summary>
	public class SiteRole
	{
		public const string Accounting = "Accounting";

        public const string AccountingDriverBilling = "Accounting.DriverBilling";
		public const string AccountingInvoicePastDues = "Accounting.InvoicePastDue";
		public const string AccountingManager = "Accounting.Manager";

		public const string ApartmentEdit = "Apartment.Edit";
		public const string ApartmentManager = "Apartment.Manager";
		public const string ApartmentView = "Apartment.View";
        public const string ApartmentPromotion = "Apartment.Promotion";
		public const string ApartmentSalesRep = "Apartment.SalesRep";
		public const string ApartmentSalesRepManager = "Apartment.SalesRepManager";

		public const string Administrators = "Administrators";

        public const string Bankruptcy = "Bankruptcy";

        public const string Contact = "Contact";
        public const string ContactManager = "Contact.Manager";

        public const string CustomerService = "CustomerService";

        public const string CustomerFollowUp = "CustomerFollowUp";
		public const string CustomerFollowUpAccountingRequest = "CustomerFollowUp.AccountingRequest";
		public const string CustomerFollowUpCorporateFollowUp = "CustomerFollowUp.CorporateFollowUp";
		public const string CustomerFollowUpDamageClaim = "CustomerFollowUp.DamageClaim";
		public const string CustomerFollowUpResidentialFollowUp = "CustomerFollowUp.ResidentialFollowUp";
		public const string CustomerFollowUpResolutions = "CustomerFollowUp.Resolutions";

        public const string CustomerTopTierView = "CustomerTopTier.View";
        public const string CustomerTopTierEdit = "CustomerTopTier.Edit";

        public const string Developer = "Developer"; //For testing pre production features on produciton.

        public const string Dispatch = "Dispatch";
		public const string DispatchReports = "Dispatch.Reports";

        public const string DriverRouteManager = "DriverRoute.Manager";
        public const string DriverRouteView = "DriverRoute.View";

        public const string Executives = "Executives";

        public const string FieldOperation = "FieldOperation";
	   
		public const string OpsAdminManager = "OpsAdmin.Manager";
		public const string OpsAdminUser = "OpsAdmin.User";

        public const string OrderResearch = "OrderResearch";

        public const string PaymentEntryView = "PaymentEntry.View";
		public const string PaymentEntryEdit = "PaymentEntry.Edit";

        public const string Personnel = "Personnel";
		public const string PersonnelBankAccount = "Personnel.BankAccount";
		public const string PersonnelAdministrator = "Personnel.Administrator";

        public const string PropertyManagementEdit = "PropertyManagement.Edit";
		public const string PropertyManagementView = "PropertyManagement.View";

        public const string ReferralRateEdit = "ReferralRate.Edit";
        public const string ReferralRateView = "ReferralRate.View";

        public const string UserAdministrator = "User.Administrator";

        public const string TimeBanksManager = "TimeBanks.Manager";


    }
}