namespace TFOF.Areas.Report.Models
{
    using System;
    using TFOF.Areas.Core.Models;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using FieldOption.Models;
    using User.Models;
    using MvcReportViewer;
    using System.Web;

    [Table("core.Report")]
    public class ReportModel : BaseModel
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string SSRSReportName { get; set; }
        
        [StringLength(150)]
        public string Category { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(30)]
        public string Format { get; set; }

        [Required]
        [StringLength(100)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string Template { get; set; }

        [StringLength(23)]
        public string ScheduledMonth { get; set; }

        [StringLength(9)]
        public string ScheduledWeek { get; set; }

        [StringLength(13)]
        public string ScheduledDays { get; set; }

        public DateTime? ScheduledTime { get; set; }

        public DateTime? LastRun { get; set; }

        [StringLength(30)]
        public string Permit { get; set; } = ReportModel.permitUserOrRole;

        public bool IsActive { get; set; }


        public virtual ICollection<ReportParameterModel> ReportParameters { get; set; }
        public virtual ICollection<ReportUserModel> ReportUsers { get; set; }
        public virtual ICollection<ReportRoleModel> ReportRoles { get; set; }
        public virtual ICollection<ReportUserActivityLogModel> ReportUserActivityLogs { get; set; }

        public bool SetParameter(string name, string value)
        {
            foreach (ReportParameterModel reportParameterModel in ReportParameters)
            {
                if (reportParameterModel.Name.Equals(name))
                {

                    reportParameterModel.DefaultValue = value;
                    return true;
                }
            }

            return false;
        }

        public bool SetParameters(HttpRequestBase request)
        {
            foreach (ReportParameterModel reportParameterModel in ReportParameters)
            {
                SetParameter(reportParameterModel.Name, request.Params.Get(reportParameterModel.Name));
            }
            if (!string.IsNullOrWhiteSpace(request.Params.Get("ReportFormat")))
            {
                Format = request.Params.Get("ReportFormat");
            }
            return true;
        }

        public static SelectList Formats = new SelectList(new List<SelectListItem>() {
            new SelectListItem() { Text = "CSV" , Value = "CSV"},
            new SelectListItem() { Text = "Excel" , Value = "Excel"},
            new SelectListItem() { Text = "PDF" , Value = "PDF"},
            new SelectListItem() { Text = "Word", Value = "Word" }
        }, "Value", "Text");

        public static Dictionary<string, string> Extensions = new Dictionary<string, string>() {
            { "CSV", "csv" },
            { "Excel", "xls" },
            { "PDF", "pdf" },
            { "Word", "doc" }
        };

        public static SelectList Categories = FieldOptionModelContext.GetByRegistration("ReportModel.Category");

        public static string permitEveryone = "Everyone";
        public static string permitUserOrRole = "UserOrRole";

        public static SelectList Permits = new SelectList(new List<SelectListItem>()
        {   
            new SelectListItem() { Value = permitEveryone, Text = "Everyone" },
            new SelectListItem() { Value = permitUserOrRole, Text = "User or Role" }
        }, "Value", "Text");
    }

    [Table("core.ReportParameter")]
    public class ReportParameterModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public virtual ReportModel Report { get; set; }

        public int Position { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Label { get; set; }

        [StringLength(30)]
        public string DataType { get; set; }

        [StringLength(150)]
        public string DefaultValue { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(200)]
        public string FieldOptionSlug { get; set; }


        public const string stringType = "string";
        public const string intType = "int";
        public const string dateTimeType = "DateTime";
        public const string boolType = "bool";

        public static SelectList dataTypes = new SelectList(new List<SelectListItem>()
        {
            new SelectListItem() { Value = stringType, Text ="String"},
            new SelectListItem() { Value = intType, Text = "Integer" },
            new SelectListItem() { Value = dateTimeType, Text = "DateTime" },
            new SelectListItem() { Value = boolType, Text ="Boolean"},

        }, "Value", "Text");
    }

    [Table("core.ReportRole")]
    public class ReportRoleModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Report")]
        [Required]
        public int ReportId { get; set; }
        public virtual ReportModel Report { get; set; }

        [StringLength(128)]
        [ForeignKey("Role")]
        [Required]
        public string RoleId { get; set; }
        public virtual RoleModel Role { get; set; }
    }

    [Table("core.ReportUser")]
    public class ReportUserModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Report")]
        [Required]
        public int ReportId { get; set; }
        public virtual ReportModel Report { get; set; }

        [ForeignKey("User")]
        [StringLength(128)]
        [Required]
        public string UserId { get; set; }
        public virtual UserModel User { get; set; }

        public bool SendEmail { get; set; }
   
        public bool SendNotification { get; set; }

    }

    [Table("core.ReportUserActivityLog")]
    public class ReportUserActivityLogModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public virtual ReportModel Report { get; set; }

        [ForeignKey("User")]
        [StringLength(128)]
        public string UserId { get; set; }
        public virtual UserModel User { get; set; }

        [StringLength(150)]
        public string Activity { get; set; }

        [StringLength(4000)]
        public string Parameters { get; set; }

        [StringLength(4000)]
        public string Errors { get; set; }

        public const string RunActivity = "Run";
        public const string ViewActivity = "View";
        public const string NotifyActivity = "Notify";
        public const string EmailActivity = "Email";

    }

    [NotMapped]
    public class ReportViewerModel
    {
        public ReportModel Report { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReportPath { get; set; }
        public string ServerUrl { get; set; }
        public IEnumerable<Parameter> Parameters { get; set; }
        public ControlSettings ControlSetting { get; set; } = new ControlSettings()
        {
            ShowToolBar = true,
            //BackColor = System.Drawing.Color.FromArgb(1, 249, 249, 249),
            BackColor = System.Drawing.Color.FromArgb(1, 240, 240, 240),
            SplitterBackColor = System.Drawing.Color.FromArgb(1, 240, 240, 240),

        };

    }

    [NotMapped]
    public class Parameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}