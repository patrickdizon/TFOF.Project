using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using TFOF.Areas.Core.Attributes;

namespace TFOF.Areas.Account.Models
{
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    //public class SetPasswordViewModel
    //{
    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "New password")]
    //    public string NewPassword { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm new password")]
    //    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }
    //}

   
    
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    //public class AddPhoneNumberViewModel
    //{
    //    [Required]
    //    [Phone]
    //    [Display(Name = "Phone Number")]
    //    public string Number { get; set; }
    //}

    //public class VerifyPhoneNumberViewModel
    //{
    //    [Required]
    //    [Display(Name = "Code")]
    //    public string Code { get; set; }

    //    [Required]
    //    [Phone]
    //    [Display(Name = "Phone Number")]
    //    public string PhoneNumber { get; set; }
    //}

    //public class ConfigureTwoFactorViewModel
    //{
    //    public string SelectedProvider { get; set; }
    //    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    //}

    
    public class UpdateProfileViewModel
    {
        [Required]
        [Display(Name = "Username/Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "The title can only be 100 characters long.")]
        [Display(Name = "Title")]
        public string Title { get; set; }
        
        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }
        public static System.Web.Mvc.SelectList TimeZones = Helpers.TimeZoneHelpers.GetTimeZones();

        [ReadOnly]
        public string Avatar { get; set; }

        public List<string> Roles { get; set; } 
    }

    public class UploadAvatarViewModel
    {
        [Required]
        [Display(Name = "Image File")]
        public HttpPostedFileBase RawAvatar { get; set; }
    }
}