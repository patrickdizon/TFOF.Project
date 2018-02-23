 

namespace TFOF.Areas.File.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TFOF.Areas.Core.Models;
    using System.Web.Mvc;
    using System.Collections.Generic;

    [Table("core.File")]
    public class FileModel : BaseModel
    {
        [Key]
        public Int64 Id { get; set; }

        [StringLength(500)]
        public string FileName { get; set; }

        [StringLength(4000)]
        public string FileLocation { get; set; }

        [StringLength(6)]
        public string Action { get; set; }

        [Required]
        [StringLength(100)]
        public string Status { get; set; }

        public static string importAction = "Import";
        public static string exportAction = "Export";
        public static string emailAction = "Print";

        public static SelectList Actions = new SelectList(new List<SelectListItem>() {
            new SelectListItem() { Text = importAction, Value = importAction},
            new SelectListItem() { Text = exportAction, Value = exportAction},
            new SelectListItem() { Text = emailAction, Value = emailAction}
        }, "Value", "Text");


        public static string pendingStatus = "Pending";
        public static string succeededStatus = "Succeeded";
        public static string failedStatus = "Failed";

        public static SelectList Statues = new SelectList(new List<SelectListItem>() {
            new SelectListItem() { Text = pendingStatus, Value = pendingStatus},
            new SelectListItem() { Text = succeededStatus, Value = succeededStatus},
            new SelectListItem() { Text = failedStatus, Value = failedStatus}
        }, "Value", "Text");

        public virtual ICollection<FileLogModel> FileLogs { get; set; }
    }
    

    [Table("core.FileLog")]
    public class FileLogModel : BaseModel
    {
        [Key]
        public Int64 Id { get; set; }

        [ForeignKey("File")]
        [Required]
        public Int64 FileId { get; set; }
        public virtual FileModel File { get; set; }

        [StringLength(128)]
        public string Activity { get; set; }

        
        public static string downloadedActivity = "Exported";
        public static string printedActivity = "Downloaded";
        public static string emailActivity = "Printed";

        public static SelectList Activities = new SelectList(new List<SelectListItem>() {
            new SelectListItem() { Text = downloadedActivity, Value = downloadedActivity},
            new SelectListItem() { Text = printedActivity, Value = printedActivity},
            new SelectListItem() { Text = emailActivity, Value = emailActivity}
        }, "Value", "Text");


       
    }


    [Table("core.FileDirectory")]
    public class FileDirectoryModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public string Path { get; set; }



        //public override WellTableModel WellTable
        //{
        //    get { return new WellTableModel() { TableName = "[]", IdField = "Id" }; }
        //}
    }
}