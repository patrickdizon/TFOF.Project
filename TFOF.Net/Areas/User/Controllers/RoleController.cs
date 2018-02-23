
namespace TFOF.Areas.User.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using TFOF.Areas.Account.Models;
    using Models;
    using TFOF.Areas.User.Forms;
    using TFOF.Areas.Core.Services;
    using Core.Attributes;
    using Core.Controllers;

    [SiteAuthorize(Roles = SiteRole.Administrators + "," + SiteRole.UserAdministrator)]
    public class RoleController :  CoreController<RoleModel>
    {
        public RoleController()
        {
            Form = new RoleForm();
            SearchForms.Add(new RoleSearchForm(Url));
            TitlePlural = "Roles";
        }

        //// GET: User/Roles/Create
        //public override ActionResult Create()
        //{
        //    Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
        //    breadcrumbs.AddCrumb("Role", Url.Action("Index", "Role"));
        //    breadcrumbs.AddCrumb("New");
        //    ViewData["Breadcrumbs"] = breadcrumbs;
        //    ViewData["Form"] = new RoleForm().Init(new RoleModel());
        //    return View();
        //}

        [HttpPost]
        // POST: User/Roles/Create
        public override ActionResult Create(RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = RoleManager.Create(new ApplicationRole(roleModel.Name));
                if (result.Succeeded)
                {
                    ApplicationRole role = RoleManager.FindByName(roleModel.Name);
                    return RedirectToAction("Edit", new { controller = "Role", id = role.Id });
                }
                TempData["Error"] = "";
                foreach (string error in result.Errors)
                {
                    TempData["Error"] += error;
                }
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        // POST: User/Roles/Delete
        public new async Task<ActionResult> Delete(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                return View("Error", new string[] { "Deleting Role did NOT succeed." });
            }
            return View("Error", new string[] { "Role not found." });
        }

        // GET: User/Roles/Edit
        public ActionResult EditUsers(string id)
        {
            RoleEditModel model = ReturnRoleFromId(id);
            
            return View(model);
        }

        // POST: User/Roles/Edit
        [HttpPost]
        public ActionResult EditUsers(RoleModificationModel model)
        {
            IdentityResult result;
            RoleEditModel editModel;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result =  UserManager.AddToRole(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        TempData["Error"] = string.Join(" | ", result.Errors);
                        editModel = ReturnRoleFromId(model.RoleId);
                        return View(editModel);
                    }
                    TempData["Success"] = "Added!";
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = UserManager.RemoveFromRole(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        TempData["Error"] = string.Join(" | ", result.Errors);
                        editModel = ReturnRoleFromId(model.RoleId);
                        return View(editModel);
                    }
                    TempData["Success"] = "Removed!";
                }
                
                editModel = ReturnRoleFromId(model.RoleId);
                return View(editModel);
            }
            TempData["Error"] = "The Model State is not Valid";
            editModel = ReturnRoleFromId(model.RoleId);
            return View(editModel);
        }
        #region (------- Helpers -------)
        void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        RoleEditModel ReturnRoleFromId(string id)
        {
            ApplicationRole role = RoleManager.FindById(id);
            string[] memberIDs = role.Users.Select(u => u.UserId).ToArray();
            IEnumerable<ApplicationUser> members = UserManager.Users.Where(u => memberIDs.Any(m => m == u.Id));
            IEnumerable<ApplicationUser> nonMembers = UserManager.Users.Except(members);
            return new RoleEditModel { Role = role, Members = members, NonMembers = nonMembers };
        }
        ApplicationUserManager UserManager { get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); } }
        ApplicationRoleManager RoleManager { get { return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>(); } }
        #endregion
    }
}