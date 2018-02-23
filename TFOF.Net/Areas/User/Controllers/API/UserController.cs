namespace TFOF.Areas.User.Controllers.API
{
    using TFOF.Areas.Core.Controllers.API;
    using TFOF.Areas.User.Models;
    using Newtonsoft.Json.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Data.Entity.Infrastructure;
    using Core.Models;
    using Core.Attributes;
    using static Core.Services.CoreService;

    [APIAuthorize(Roles = SiteRole.Administrators + "," + SiteRole.UserAdministrator)]
    public class RoleController : CoreController<RoleModel> { }

    [APIAuthorize(Roles = SiteRole.Administrators + "," + SiteRole.UserAdministrator)]
    public class UserController : CoreController<UserModel> { }

    [APIAuthorize(Roles = SiteRole.Administrators + "," + SiteRole.UserAdministrator)]
    public class UserRoleController : CoreController<UserRoleModel>
    {
        //PUT: User/UserRole/API/5
        [ResponseType(typeof(void))]
        public override IHttpActionResult Put(string id, string method, JObject jsonObject)
        {
            BaseModelContext<UserModel> userdb = new BaseModelContext<UserModel>();
            BaseModelContext<RoleModel> roledb = new BaseModelContext<RoleModel>();
            UserModel user = userdb.Models.Find(id);

            ServiceResult serviceResult = new ServiceResult();
            if (user != null)
            {
                if (method.Equals("Toggle"))
                {
                    string roleId = (string)jsonObject.GetValue("RoleId");
                    RoleModel role = roledb.Models.Find(roleId);
                    UserRoleModel userRole = DB.Models.Find(id, roleId);

                    if (userRole == null && role != null)
                    {
                        userRole = new UserRoleModel();
                        userRole.RoleId = roleId;
                        userRole.UserId = id;
                        userRole.CreatedById = UserId;
                        DB.Models.Add(userRole);
                        serviceResult.Success($"{role.Name} enabled.");
                    } else if (userRole != null)
                    {
                        DB.Models.Remove(userRole);
                        serviceResult.Success($"{role.Name} disabled.");
                    }
                    try
                    {
                        DB.SaveChanges();
                        
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        serviceResult.Error($"An error occurred while. Please refresh and try again.");
                    }
                }
                
            }
            
            return Ok(serviceResult);
        }
    }
}