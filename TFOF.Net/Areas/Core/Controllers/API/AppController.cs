namespace TFOF.Areas.Core.Controllers.API
{
    using Core.Attributes;
    using TFOF.Areas.Core.Models;

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class AppController : CoreController<AppModel> { }

    [SiteAuthorize(Roles = SiteRole.Administrators)]
    public class AppFieldController : CoreController<AppFieldModel> { }
}