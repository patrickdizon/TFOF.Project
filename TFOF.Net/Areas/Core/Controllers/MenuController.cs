namespace TFOF.Areas.Core.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using System.IO;

    using TFOF.Areas.Core.Models;
    using TFOF.Areas.Core.Forms;
    using System.Collections.Generic;
    using Services;
    using Attributes;

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class MenuController : CoreController<MenuModel>
    {
        //private AppModelContext db = new AppModelContext();
        public MenuController()
        {
            Form = new MenuForm();
            CanDelete = true;
            TitlePlural = "Menus";
            ForeignKey = "ParentId";
            ReturnTo = new ReturnTo(this, "Edit", "ParentId", "Label", new string[] { "Delete" });

            
            SearchForms.Add(new MenuSearchForm(Url));
        }
        
    }
    
}
