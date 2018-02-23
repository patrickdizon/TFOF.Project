namespace TFOF.Areas.App.Controllers.API
{
    using Core.Attributes;
    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.Core.Models;

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class MenuController : CoreController<MenuModel> { }
}