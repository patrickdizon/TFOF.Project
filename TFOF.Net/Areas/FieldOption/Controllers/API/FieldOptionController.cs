namespace TFOF.Areas.FieldOption.Controllers.API
{
    using Core.Attributes;
    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.FieldOption.Models;

    [APIAuthorize(Roles = SiteRole.Administrators)]
    public class FieldOptionController : CoreController<FieldOptionModel> { }
    
}