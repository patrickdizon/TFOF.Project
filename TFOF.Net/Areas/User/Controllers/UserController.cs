namespace TFOF.Areas.User.Controllers
{
    using Account.Services;
    using Core.Attributes;
    using Core.Models;
    using TFOF.Areas.Core.Controllers;
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Core.Services;
    using TFOF.Areas.User.Forms;
    using TFOF.Areas.User.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    [SiteAuthorize(Roles = SiteRole.Administrators + "," + SiteRole.UserAdministrator)]
    public class UserController : CoreController
    {
        UserModelContext db = new UserModelContext();

        // GET: User/User
        public ActionResult Index()
        {
           
            ViewData["SearchForm"] = new UserSearchForm(Url);


            return View();
        }

        // GET: User/User/Details/id
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModel model = db.Users.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            db.Entry(model).Collection("UserRoles").Load();
            IEnumerable<string> userRoles = from role in model.UserRoles select role.Role.Id;
            var roles = db.Roles.OrderBy(o => o.Name).ToList();
            List<SelectListItem> AvailableRoles = new List<SelectListItem>();
            foreach (RoleModel r in roles)
            {
                AvailableRoles.Add(new SelectListItem() { Value = r.Id, Text = r.Name, Selected = (userRoles.Contains(r.Id) ? true : false) });
            }
            ViewData["Breadcrumbs"] = new Breadcrumbs(Url).AddCrumb("Users", Url.Action("Index", "User")).AddCrumb("Details");
            ViewData["AvailableRoles"] = new MultiSelectList(AvailableRoles, "Value", "Text", userRoles);
            return View(model);
        }
        // GET: User/User/Edit/id
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModel model = db.Users.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            //ViewBag.TimeZones = Account.Helpers.TimeZoneHelpers.GetTimeZones();
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Users", Url.Action("Index", "User"));
            breadcrumbs.AddCrumb(model.FullName, Url.Action("Details", "User", new { id = model.Id }));
            breadcrumbs.AddCrumb("Edit");
            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Form"] = new UserForm().Init(model);
            return View(model);
        }
        // POST: User/User/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.Id);
                System.Diagnostics.Debug.WriteLine(user);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Title = model.Title;
                    user.TimeZone = model.TimeZone;

                    IdentityResult result = UserManager.Update(user);
                    if (!result.Succeeded)
                    {
                        TempData["Error"] = string.Join(" | ", result.Errors);
                    }
                    else
                    {
                        TempData["Success"] = "Saved!";
                        return RedirectToAction("Details", "User", new { area = "User", id = user.Id });
                       
                    }
                }
            }
            // We are re-displaying the view with the success or error message
            //ViewBag.TimeZones = Account.Helpers.TimeZoneHelpers.GetTimeZones();
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Users", Url.Action("Index", "User"));
            breadcrumbs.AddCrumb(model.FullName, Url.Action("Details", "User", new { id = model.Id }));
            breadcrumbs.AddCrumb("Edit");
            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Form"] = new UserForm().Init(model);
            return View(model);
        }
        public ActionResult Create()
        {
            CreateViewModel model = new CreateViewModel();
            //model.TimeZones = Account.Helpers.TimeZoneHelpers.GetTimeZones();
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Users", Url.Action("Index", "User"));
            breadcrumbs.AddCrumb("Create");
            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Form"] = new CreateUserForm().Init(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Account.Models.ApplicationUser { UserName = model.Email, Email = model.Email, LastName = model.LastName, FirstName = model.FirstName, Title = model.Title, TimeZone = model.TimeZone, LastLogin = DateTime.Parse("1/1/1") };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Details", "User", new { area = "User", id = user.Id });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            //model.TimeZones = Account.Helpers.TimeZoneHelpers.GetTimeZones();
            Breadcrumbs breadcrumbs = new Breadcrumbs(Url);
            breadcrumbs.AddCrumb("Users", Url.Action("Index", "User"));
            breadcrumbs.AddCrumb("Create");
            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Form"] = new CreateUserForm().Init(model);
            return View(model);
        }
        public async Task<ActionResult> ResetPassword(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User could not be found";
                return RedirectToAction("Index", "User");
            }

            if (await AccountService.ResetPassword(user, ControllerContext, Request, UserManager, Url))
            {
                TempData["Success"] = "An email has been sent to " + user.FullName + " with instructions on how to reset their account password.";
            } else
            {
                TempData["Error"] = "An error occurred while resetting the password for " + user.FullName + ".";
            }
            return RedirectToAction("Details", "User", new { area = "User", id = id });
        }

  

        ApplicationUserManager UserManager { get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); } }
        ApplicationRoleManager RoleManager { get { return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>(); } }
        void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}