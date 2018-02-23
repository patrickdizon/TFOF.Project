namespace TFOF.Areas.Report.Controllers.API
{
    using Core.Attributes;
    using Core.Controllers.API;
    using Models;

    [APIAuthorize(Roles = SiteRole.Administrators)]
    public class ReportController : CoreController<ReportModel> { }

    [APIAuthorize(Roles = SiteRole.Administrators)]
    public class ReportParameterController : CoreController<ReportParameterModel> { }

    [APIAuthorize(Roles = SiteRole.Administrators)]
    public class ReportUserController : CoreController<ReportUserModel> { }

    [APIAuthorize(Roles = SiteRole.Administrators)]
    public class ReportRoleController : CoreController<ReportRoleModel> { }

    [APIAuthorize(Roles = SiteRole.Administrators)]
    public class ReportUserActivityLogController : CoreController<ReportUserActivityLogModel> { }

}
