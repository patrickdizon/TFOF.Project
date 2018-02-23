namespace TFOF.Areas.Account.Forms
{
    using Account.Models;
    using TFOF.Areas.Core.Forms;

    public class ChangePasswordForm : Form
    {
        public ChangePasswordForm()
        {
            FormTitle = "Change Password";
            SaveButtonText = "Change Password";
            Fields.Add(new PasswordField() { Name = "OldPassword", ClassNames = "col-sm-6"});
            Fields.Add(new BlankCell() { ClassNames = "col-sm-6" });
            Fields.Add(new PasswordField() { Name = "NewPassword", ClassNames = "col-sm-6" });
            Fields.Add(new BlankCell() { ClassNames = "col-sm-6" });
            Fields.Add(new PasswordField() { Name = "ConfirmPassword", ClassNames = "col-sm-6" });
        }
    }

    public class UpdateProfileForm : Form
    {
        public UpdateProfileForm()
        {
            FormTitle = "Update Profile";
            Fields.Add(new ReadOnlyField() { Name = "Email", Label = "Username/Email"});
            Fields.Add(new CharField() { Name = "FirstName", ClassNames = "col-sm-6" });
            Fields.Add(new CharField() { Name = "LastName", ClassNames = "col-sm-6" });
            Fields.Add(new CharField() { Name = "Title" });
            Fields.Add(new CharField() { Name = "TimeZone", Options = UpdateProfileViewModel.TimeZones });
        }
    }


}