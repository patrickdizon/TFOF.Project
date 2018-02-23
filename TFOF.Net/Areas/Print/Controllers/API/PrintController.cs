 

namespace TFOF.Areas.Print.Controllers.API
{

    using System.Web.Http;
    using System.Web.Http.Description;

    using Newtonsoft.Json.Linq;

    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.Print.Models;

    public class PrintController : CoreController<PrintModel> { }

    public class PrintLogController : CoreController<PrintLogModel> { }

    public class PrinterController : CoreController<PrinterModel> { }
}