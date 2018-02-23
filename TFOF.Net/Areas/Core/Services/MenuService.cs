namespace TFOF.Areas.Core.Services
{
    using System.Web.Mvc;
    using System.Collections.Generic;
    using Attributes;
    using Models;
    using System.Linq;
    using System.Security.Principal;

    public class MainNavigation
    {

        //Draw Main Navigation
        public List<AbstractNavigationItem> NavigationItems { get; set; }

        public MainNavigation(UrlHelper url, IPrincipal user)
        {
            this.NavigationItems = new List<AbstractNavigationItem>();
            //Accounting//

            foreach(MenuModel menu in new BaseModelContext<MenuModel>().Models.Where(p => p.ParentId == null).OrderBy(g => g.GroupNumber).OrderBy(p => p.Position))
            {
                string link = url.Action(menu.Action, menu.Controller, new { area = menu.Area });
                NavigationItem item = new NavigationItem() {
                    Label = menu.Label,
                    Icon = menu.Icon,
                    Link = (!string.IsNullOrWhiteSpace(link) ? link : null)
                };
                foreach (MenuModel subMenu in menu.SubMenus.Where(e => e.Environment != null).OrderBy(g => g.GroupNumber).OrderBy(p => p.Position))
                {
                    if ( subMenu.Environment.Equals(MenuModel.prodEnvironment) || user.IsInRole(SiteRole.Administrators))
                    {
                        NavigationItem subItem = new NavigationItem()
                        {
                            Label = subMenu.Label  + (!subMenu.Environment.Equals(MenuModel.prodEnvironment) ? " (" + subMenu.Environment + ")" : ""),
                            Link = url.Action(subMenu.Action, subMenu.Controller, new { area = subMenu.Area }),
                            Role = subMenu.Role,
                            Group = subMenu.GroupNumber
                        };
                        item.SubNavigationItems.Add(subItem);
                    }
                }
                //if (item.SubNavigationItems.Count > 0)
                // {
                    NavigationItems.Add(item);
                //}
            }

            //var accounting = new NavigationItem() { Label = "Accounting", Icon = "fa-line-chart" };

            //// accounting.SubNavigationItems.Add(
            ////     new NavigationItem() { Label = "Bankruptcy Codes", Link = url.Action("Index", "BankruptcyCode", new { area = "Bankruptcy" }), Role = SiteRole.Accounting}
            //// );
            //accounting.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Bankruptcy Notices", Link = url.Action("Index", "BankruptcyNotice", new { area = "Bankruptcy" }), Role = SiteRole.Accounting}
            //);
            //accounting.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Gift Certificates", Link = url.Action("Index", "GiftCertificate", new { area = "Gift" }), Role = SiteRole.Accounting}
            //);
            //accounting.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Money Orders", Link = url.Action("Index", "MoneyOrder", new { area = "Gift" }), Role = SiteRole.Accounting}
            //);
            //accounting.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Payments", Link = url.Action("Index", "Payment", new { area = "Payment" }), Role = SiteRole.Accounting, Group = 2}
            //);
            //accounting.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Payment Batches", Link = url.Action("Index", "PaymentBatch", new { area = "Payment" }), Role = SiteRole.Accounting, Group = 2 }
            //);
            //accounting.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Payment Batch Entries", Link = url.Action("Index", "PaymentBatchEntry", new { area = "Payment" }), Role = SiteRole.Accounting, Group = 2 }
            //);
            ////Customers//
            //var customer = new NavigationItem() { Label = "Customer", Icon = "fa-heart" };
            
            //customer.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Customers", Link = url.Action("Index", "Customer", new { area = "Customer" }), Role = SiteRole.CustomerService}
            //);
            //customer.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Issues", Link = url.Action("Index", "CustomerIssue", new { area = "CustomerIssue" }), Role = SiteRole.CustomerService}
            //);
            
            //customer.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Invoices", Link = url.Action("Index", "Invoice", new { area = "Invoice" }), Role = SiteRole.CustomerService}
            //);
            //customer.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Leases", Link = url.Action("Index", "Lease", new { area = "Lease" }), Role = SiteRole.CustomerService}
            //);
            //customer.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Lease Addresses", Link = url.Action("Index", "LeaseAddress", new { area = "Lease" }), Role = SiteRole.CustomerService}
            //);
            ////Orders//

            ////Collections//

            ////Dispatch Menu//
            //var dispatch = new NavigationItem() { Label = "Dispatch", Icon = "fa-truck fa-flip-horizontal" };
            
            //dispatch.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Personnel", Link = url.Action("Index", "Personnel", new { area = "Personnel" }), Role = SiteRole.Dispatch}
            //);
            //dispatch.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Schedule", Link = url.Action("Index", "Schedule", new { area = "Schedule" }), Role = SiteRole.Dispatch}
            //);

            ////Commercial//

            ////Inventory//
            //var inventory = new NavigationItem() { Label = "Inventory", Icon = "fa-barcode" };
            

            //inventory.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Equipments", Link = url.Action("Index", "Equipment", new { area = "Equipment" }), Role = SiteRole.FieldOperation}
            //);
            //inventory.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Equipment Types", Link = url.Action("Index", "EquipmentType", new { area = "Equipment" }), Role = SiteRole.FieldOperation}
            //);
            //inventory.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Equipment Missing History", Link = url.Action("Index", "EquipmentMissingHistory", new { area = "Equipment" }), Role = SiteRole.FieldOperation}
            //);

            ////Marketing //
            //var marketing = new NavigationItem() { Label = "Marketing", Icon = "fa-bullhorn" };
            
            //marketing.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Apartments", Link = url.Action("Index", "Apartment", new { area = "Apartment" }), Role = SiteRole.Marketing}
            //);
            //marketing.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Houses", Link = url.Action("Index", "House", new { area = "Apartment" }), Role = SiteRole.Marketing}
            //);
            //marketing.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Property Management Companies", Link = url.Action("Index", "PropertyManagement", new { area = "PropertyManagement" }), Role = SiteRole.Marketing}
            //);
            //marketing.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Apartment Mailer Opt Outs", Link = url.Action("Index", "ApartmentMailerOptOut", new { area = "Apartment" }), Role = SiteRole.Marketing}
            //);
            //marketing.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Referrals", Link = url.Action("Index", "Referral", new { area = "Referral" }), Role = SiteRole.Marketing}
            //);
            //marketing.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Referrers", Link = url.Action("Index", "Referrer", new { area = "Referral" }), Role = SiteRole.Marketing}
            //);
            
            ////Admin Menu//
            //var admin = new NavigationItem() { Label = "Admin", Icon = "fa-cog" , Role = SiteRole.Administrators };
            
            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Contacts", Link = url.Action("Index", "Contact", new { area = "Contact" }), Role = SiteRole.Administrators }
            //);
            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Field Options", Link = url.Action("Index", "FieldOption", new { area = "FieldOption" }), Role = SiteRole.Administrators }
            //);
            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Payables", Link = url.Action("Index", "Payable", new { area = "Payable" }), Role = SiteRole.Administrators }
            //);
            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Regions", Link = url.Action("Index", "Region", new { area = "Region" }), Role = SiteRole.Administrators }
            //);

            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "EmailLog Internal", Link = url.Action("Index", "EmailInternal", new { area = "Email" }), Role = SiteRole.Administrators }
            //);
            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "PDFs", Link = url.Action("Index", "PDF", new { area = "PDF" }), Role = SiteRole.Administrators, Group = 2}
            //);
            //// admin.SubNavigationItems.Add(new DividerItem());
            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Users", Link = url.Action("Index", "User", new { area = "User" }), Role = SiteRole.UserAdministrator, Group = 2}
            //);

            //admin.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "User Roles", Link = url.Action("Index", "Roles", new { area = "User" }), Role = SiteRole.Administrators, Group = 2}
            //);

            //var report = new NavigationItem() { Label = "Report", Icon = "fa-file-o" };
            

            //report.SubNavigationItems.Add(
            //    new NavigationItem() { Label = "Reports", Link = url.Action("Index","Report", new { area = "Report" }), Role = "Report"}
            //);


            ////Add in order of priority: Asc except admin
            //NavigationItems.Add(accounting);
            //NavigationItems.Add(customer);
            //NavigationItems.Add(dispatch);
            //NavigationItems.Add(inventory);
            //NavigationItems.Add(marketing);
            //NavigationItems.Add(report);
            //NavigationItems.Add(admin);
        }
        
    }

    public abstract class AbstractNavigationItem
    {

        public bool IsDivider { get; set; }
        public string Link { get; set; }
        public string Label { get; set; }
        public bool IsAuthenticated { get; set; } = true;
        public string Role { get; set; }
        public string Icon { get; set; }
        public int Group { get; set; } = 1;
        public string Environment { get; set; } = "DEV"; //DEV, UAT, PROD

        public List<AbstractNavigationItem> SubNavigationItems;

        public AbstractNavigationItem()
        {        }

        public AbstractNavigationItem(string label = null, string link = null, bool isAuthenticated = true, string roles = null, string icon = null)
        {
            Link = link;
            Label = label;
            Role = roles;
            IsAuthenticated = isAuthenticated;
            Icon = icon;
            SubNavigationItems = new List<AbstractNavigationItem>();
        }


        public int Groups
        {
            get
            {
                int maxGroup = 0;
                foreach (AbstractNavigationItem item in SubNavigationItems)
                {
                    maxGroup = (item.Group > maxGroup) ? item.Group : maxGroup;
                }
                return maxGroup;
            }
            set { }
        }
    }


    public class NavigationItem : AbstractNavigationItem
    {
        public NavigationItem(string label = null, string link = null, bool isAuthenticated = true, string roles = null, string icon = null) : 
            base(label, link, isAuthenticated, roles, icon)
        {

        }
    }

    public class DividerItem : AbstractNavigationItem
    {
        public DividerItem() {
            this.IsDivider = true;
        }
    }

    
    public class Breadcrumbs
    {
        public List<AbstractNavigationItem> Crumbs;

        public Breadcrumbs(UrlHelper url)
        {
            Crumbs = new List<AbstractNavigationItem>();
            Crumbs.Add(new NavigationItem() { Icon = "fa fa-home", Link = url.Action("Index", "Home", new { area = "Home" }) });
        }
        public Breadcrumbs AddCrumb(string label, string link = null)
        {
            Crumbs.Add(new NavigationItem() { Label = label, Link = link });
            return this;
        }

        
    }
}