namespace TFOF.Areas.Core.Controllers.API
{
    using Core.Attributes;
    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.Core.Models;

    [APIAuthorize]
    public class AuditController : CoreController<AuditModel> { }
    
}