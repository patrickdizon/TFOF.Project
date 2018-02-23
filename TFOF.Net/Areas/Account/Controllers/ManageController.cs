namespace TFOF.Areas.Account.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using System.IO;
    using System;


    using TFOF.Areas.Account.Models;
    using TFOF.Areas.Account.Forms;
    using System.Collections.Generic;
    using System.Drawing;
    using Core.Models;
    using Core.Helpers;
    using Core.Attributes;
    using Core.Controllers;
    using User.Models;

    [SiteAuthorize]
    public class AvatarController: CoreController<UserModel>
    {

        //GET: /Account/Manage/Avatar/324908234-234823-0284234
        public ActionResult Image(string id)
        {
            FileHelper fileModel = new FileHelper(id + ".jpg", id);
            if (!fileModel.Exists())
            {
                BaseModelContext<UserModel> userDb = new BaseModelContext<UserModel>();
                UserModel user = userDb.Models.Find(id);
                if(user != null && !string.IsNullOrWhiteSpace(user.Avatar))
                {
                    fileModel.Write(Convert.FromBase64String(user.Avatar));
                }
                else
                {
                    fileModel.FilePathAndName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images") + "/silhouette_md.png";
                }
            }

            return File(fileModel.GetContent(), "image/jpeg");
        }

    }

    [SiteAuthorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        //
        // GET: /Account/Manage/UpdateProfile
        public ActionResult UpdateProfile()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            UpdateProfileViewModel model = new UpdateProfileViewModel();
            model.Email = user.Email;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Title = user.Title;
            model.TimeZone = user.TimeZone;
            model.Avatar = user.Avatar;
            model.Roles = new List<string>();
            foreach(var role in RoleManager.Roles.ToList())
            {
                foreach(var u in role.Users) {
                    if(u.UserId == user.Id)
                    {
                        model.Roles.Add(role.Name);
                    }
                }
            }
            ViewData["Form"] = new UpdateProfileForm().Init(model, User);
            return View(model);
        }

        //
        // POST: /Account/Manage/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UpdateProfileViewModel model)
        {
            ViewData["Form"] = new UpdateProfileForm().Init(model, User);
            if (ModelState.IsValid)
            {

                var user = UserManager.FindById(User.Identity.GetUserId());
                System.Diagnostics.Debug.WriteLine(user);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Title = model.Title;
                    user.TimeZone = model.TimeZone;

                    IdentityResult result = UserManager.Update(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Could not update your profile.");
                    }
                    else
                    {
                        this.TempData["Success"] = "Saved!";
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        ////
        //// POST: /Account/Manage/RemoveLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message;
        //    var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("ManageLogins", new { Message = message });
        //}

        ////
        //// GET: /Account/Manage/AddPhoneNumber
        //public ActionResult AddPhoneNumber()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Manage/AddPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // Generate the token and send it
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
        //    if (UserManager.SmsService != null)
        //    {
        //        var message = new IdentityMessage
        //        {
        //            Destination = model.Number,
        //            Body = "Your security code is: " + code
        //        };
        //        await UserManager.SmsService.SendAsync(message);
        //    }
        //    return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        //}

        ////
        //// POST: /Account/Manage/EnableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EnableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        ////
        //// POST: /Account/Manage/DisableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DisableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        ////
        //// GET: /Account/Manage/VerifyPhoneNumber
        //public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        //{
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
        //    // Send an SMS through the SMS provider to verify the phone number
        //    return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        //}

        ////
        //// POST: /Account/Manage/VerifyPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
        //    }
        //    // If we got this far, something failed, redisplay form
        //    ModelState.AddModelError("", "Failed to verify phone");
        //    return View(model);
        //}

        ////
        //// POST: /Account/Manage/RemovePhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemovePhoneNumber()
        //{
        //    var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
        //    if (!result.Succeeded)
        //    {
        //        return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        //    }
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        //}

        // GET: /Account/Manage/UploadAvatar
        public ActionResult UploadAvatar()
        {
            return View();
        }

        //
        // POST: /Account/Manage/UploadAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAvatar(UploadAvatarViewModel model)
        {

            if (Request.Files["RawAvatar"].HasFile() && IsValidImageType(Request.Files["RawAvatar"].ContentType))
            {
                var user = UserManager.FindById(User.Identity.GetUserId());

                string filename = Path.GetFileName(Request.Files["RawAvatar"].FileName);
                FileHelper fileModel = new FileHelper(filename, user.Id);
                Request.Files["RawAvatar"].SaveAs(Path.Combine(fileModel.FilePath, filename));

                Bitmap image = (Bitmap)Image.FromFile(fileModel.FilePathAndName);



                FileHelper thumbNailFileModel = new FileHelper(user.Id + "_avatar.jpg", user.Id);
                if (image.ProcessAvatar(thumbNailFileModel.FilePathAndName) != null)
                {
                    string base64Avatar = Convert.ToBase64String(thumbNailFileModel.GetContent());

                    if (user != null && !string.IsNullOrWhiteSpace(base64Avatar))
                    {
                        user.Avatar = base64Avatar;

                        IdentityResult result = UserManager.Update(user);
                        if (!result.Succeeded)
                        {
                            TempData["Success"] = "An error occured. Please try again.";
                        }
                        else
                        {
                            TempData["Success"] = "Saved!";

                            //Delete existing avatar on disk so that UI reflects new avatar
                            FileHelper existingFileModel = new FileHelper(user.Id + ".jpg", user.Id);
                            try
                            {
                                existingFileModel.Delete();
                            }
                            catch
                            {
                                //do nothing
                            }
                            
                        }
                    }
                }
                image.Dispose();
                thumbNailFileModel.Delete();
                fileModel.Delete();
            }
            else
            {
                TempData["Error"] = "Avatars can only be PNG, JPG or GIF images.";
            }
            
            return Redirect(Url.Action("UpdateProfile", "Manage"));
        }

        public ActionResult DeleteAvatar()
        {
            BaseModelContext<UserModel> userDb = new BaseModelContext<UserModel>();
            UserModel user = userDb.Models.Find(User.Identity.GetUserId());
            if(user != null)
            {
                user.Avatar = null;
                userDb.Entry(user);
                userDb.SaveChanges();
                FileHelper fileModel = new FileHelper(user.Id + ".jpg", user.Id);
                fileModel.Delete();

            }
            return Redirect(Url.Action("UpdateProfile", "Manage"));
        }


        //
        // GET: /Account/Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            ViewData["Form"] = new ChangePasswordForm().Init(null, User);
            return View();
        }

        //
        // POST: /Account/Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewData["Form"] = new ChangePasswordForm().Init(model, User);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    user.LastLogin = DateTime.Now;
                    IdentityResult updateResult = UserManager.Update(user);
                    TempData["Success"] = "You have successfully changed youd password.";
                }
                return RedirectToAction("ChangePassword");
            }
            AddErrors(result);
            return View(model);
        }

        ////
        //// GET: /Account/Manage/SetPassword
        //public ActionResult SetPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Manage/SetPassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        //        if (result.Succeeded)
        //        {
        //            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //            if (user != null)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                user.LastLogin = DateTime.Now;
        //                IdentityResult updateResult = UserManager.Update(user);
        //            }
        //            return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        
        // GET: /Account/Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        ////
        //// POST: /Account/Manage/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new TFOF.Areas.Account.Controllers.AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        //}

        ////
        //// GET: /Manage/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _userManager != null)
        //    {
        //        _userManager.Dispose();
        //        _userManager = null;
        //    }

        //    base.Dispose(disposing);
        //}

        

       

        #region Helpers
        private bool IsValidImageType(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                case "image/gif":
                case "image/png":
                    return true;
                default:
                    return false;
            }
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        ApplicationRoleManager RoleManager { get { return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>(); } }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}