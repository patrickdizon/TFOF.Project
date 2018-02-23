namespace TFOF.Areas.Core.Controllers
{
    using TFOF.Areas.Core.Models;
    using TFOF.Areas.Core.Forms;
    using Attributes;
    using System.Web.Mvc;

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class AuditController : CoreController<AuditModel>
    {
        public AuditController()
        {
            TitlePlural = "Audits";
            SearchForms.Add(new AuditSearchForm(Url));
        }
        
    }
}
