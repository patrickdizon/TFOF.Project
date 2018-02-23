using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using TFOF.Areas.Account.Models;
using System.Collections;
using System.Web.Security;
using TFOF.Areas.Core.Models;

namespace TFOF.Areas.User.Models
{
    public class UserModelContext : DbContext
    {
        public UserModelContext() : base("name=BaseModelContext")
        { }

        public virtual DbSet<UserModel> Users { get; set; }
        public virtual DbSet<RoleModel> Roles { get; set; }
        public virtual DbSet<UserRoleModel> UserRoles { get; set; }
    }

    /// <summary>
    /// The UserModel Class is not meant to duplicate the AspNetIdentity User (which extend Identity User)
    /// but to allow us to get the information we need while insulating the adminstrator from personal
    /// information specific to the user whos account is being viewed or modified.
    /// </summary>
    [Table("dbo.AspNetUsers")]
    public class UserModel
    {
        [Key]
        public string Id { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(256)]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        [StringLength(256)]
        public string FirstName { get; set; }

        public string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Time Zone")]
        [StringLength(50)]
        public string TimeZone { get; set; }

        public string Avatar { get; set; }

        [Display(Name = "Last Login")]
        public DateTime LastLogin { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Created { get; set; }

        //Disable preload by eliminating virtual.
        public ICollection<UserRoleModel> UserRoles { get; set; }


        public static string RandomPassword()
        {
           string uppercases = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
           string lowercases = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            return uppercases.Split(',')[new Random().Next(0, 25)] +
                Membership.GeneratePassword(7, 2) +
                lowercases.Split(',')[new Random().Next(0, 25)] +
                (new Random().Next()).ToString();
        }
    }

    [Table("dbo.AspNetRoles")]
    public class RoleModel
    {
        [Key]
        [StringLength(128)]
        public string Id { get; set; }
        
        [StringLength(256)]
        public string Name { get; set; }
    
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Created { get; set; }

        public virtual ICollection<UserRoleModel> UserRoles { get; set; }
    }

    [Table("dbo.AspNetUserRoles")]
    public class UserRoleModel
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual UserModel User { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(128)]
        [ForeignKey("Role")]
        public string RoleId { get; set; }
        public virtual RoleModel Role { get; set; }

        [StringLength(128)]
        public string CreatedById { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Created { get; set; }

        public UserModel GetCreator()
        {
            return new BaseModelContext<UserModel>().Models.Find(CreatedById);
        }

    }

    public class RoleEditModel
    {
        public ApplicationRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        [Required]
        [StringLength(256)]
        public string RoleName { get; set; }
        [Required]
        public string RoleId { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }

    public class UserDetailsViewModel
    {
        public UserModel User { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }

    public class CreateViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(256)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(256)]
        public string LastName { get; set; }

        [Display(Name = "Title")]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Time Zone")]
        [StringLength(50)]
        public string TimeZone { get; set; } = "Central Standard Time";
        public IEnumerable<System.Web.Mvc.SelectListItem> TimeZones { get; set; }
    }

    [Table("pdx_passwords")]
    public class ParadoxUserModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        [Column("Whose Password")]
        public string Username { get; set; }

        [StringLength(255)]
        [Column("ThePassword")]
        public string Password { get; set; }


        [StringLength(255)]
        public string Initials { get; set; }

        [StringLength(255)]
        [Column("First Name")]
        public string FirstName { get; set; }

        [StringLength(255)]
        [Column("Last Name")]
        public string LastName { get; set; }

        public DateTime? LastChanged { get; set; }

        public int? ChangeIntervalInDays { get; set; }

        public bool? AvailableForQueue { get; set; }

    }


    public class RightsTableModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        [Column("WhichEmployee")]
        public string UserName { get; set; }


        [StringLength(255)]
        [Column("Authorized Services")]
        public string Role { get; set; }

    }



}