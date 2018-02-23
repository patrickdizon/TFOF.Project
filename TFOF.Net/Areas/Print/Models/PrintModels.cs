namespace TFOF.Areas.Print.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using TFOF.Areas.Core.Models;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using FieldOption.Models;
    using System.Linq;

    public class PrintModelContext : BaseModelContext
    {
        public static SelectList GetPrinters(bool isActiveOnly = true)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            BaseModelContext<PrinterModel> db = new BaseModelContext<PrinterModel>();

            List<PrinterModel> printers = db.Models.OrderBy(o => o.Name).ThenBy(o => o.PrinterTray.Index).ToList();

            if(isActiveOnly)
            {
                printers = printers.Where(p => p.IsActive == true).ToList();
            }
            listItems.Add(new SelectListItem { Text = null, Value = null });
            foreach (PrinterModel item in printers)
            {
                listItems.Add(new SelectListItem { Text = item.Description + (item.PrinterTray != null && !string.IsNullOrWhiteSpace(item.PrinterTray.Name) ? " - " + item.PrinterTray.Name : ""), Value = item.Id.ToString() });
            }
            return new SelectList(listItems, "Value", "Text");
        }

        public static SelectList GetPrinterTrays(int printerId)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            BaseModelContext<PrinterTrayModel> db = new BaseModelContext<PrinterTrayModel>();

            List<PrinterTrayModel> printerTrays = db.Models.Where(p => p.PrinterId == printerId).OrderBy(o => o.Index).ToList();

            listItems.Add(new SelectListItem { Text = null, Value = null });
            foreach (PrinterTrayModel item in printerTrays)
            {
                listItems.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return new SelectList(listItems, "Value", "Text");
        }
    }

    [Table("core.Print")]
    public class PrintModel : BaseModel
    {
        [Key]
        public Int64 Id { get; set; }

        [StringLength(128)]        
        public string BatchId { get; set; }

        [StringLength(500)]
        public string EntityName { get; set; }

        [StringLength(128)]
        public string PrimaryKey { get; set; }

        [Required]
        [StringLength(4000)]
        public string FileLocation { get; set; }

        [Required]
        [ForeignKey("Printer")]
        public int PrinterId { get; set; }
        public virtual PrinterModel Printer { get; set; }

        public DateTime? PrintedDateTime { get; set; }

        [Required]
        public int RetryCount { get; set; }

        [StringLength(100)]
        [Required]
        public string Status { get; set; } = "Queued";

        public bool DeleteFile { get; set; }

        public string ErrorMessage { get; set; }

        public static string newStatus = "New"; 
        public static string queuedStatus = "Queued";
        public static string printedStatus = "Printed";
        public static string failedStatus = "Failed";
        public static string archivedStatus = "Archived";


        public static SelectList Statuses = new SelectList(new List<SelectListItem>() {
            new SelectListItem() { Text = newStatus, Value = newStatus},
            new SelectListItem() { Text = queuedStatus, Value = queuedStatus},
            new SelectListItem() { Text = printedStatus, Value = printedStatus},
            new SelectListItem() { Text = failedStatus, Value = failedStatus},
            new SelectListItem() { Text = archivedStatus, Value = archivedStatus}
        }, "Value", "Text");

        public static SelectList Printers = FieldOptionModelContext.GetByRegistration("PrintModel.PrinterName");
        public static string pdfPrinter = "PDF";
    }

    [Table("core.PrintLog")]
    public class PrintLogModel : BaseModel
    {
        [Key]
        public Int64 Id { get; set; }

        public Int64 PrintId { get; set; }

        [StringLength(500)]
        public string EntityName { get; set; }

        [StringLength(128)]
        public string PrimaryKey { get; set; }

        [Required]
        [StringLength(4000)]
        public string FileLocation { get; set; }

        [Required]
        [ForeignKey("Printer")]
        public int PrinterId { get; set; }
        public virtual PrinterModel Printer { get; set; }

        public DateTime? PrintedDateTime { get; set; }

        [StringLength(100)]
        [Required]
        public string Status { get; set; } = "Queued";

        public int RetryCount { get; set; }

        public bool DeleteFile { get; set; }

        public string ErrorMessage { get; set; }
        public static string newStatus = "New";
        public static string queuedStatus = "Queued";
        public static string printedStatus = "Printed";
        public static string failedStatus = "Failed";
        public static string archivedStatus = "Archived";


        public static SelectList Statuses = new SelectList(new List<SelectListItem>() {
            new SelectListItem() { Text = newStatus, Value = newStatus},
            new SelectListItem() { Text = queuedStatus, Value = queuedStatus},
            new SelectListItem() { Text = printedStatus, Value = printedStatus},
            new SelectListItem() { Text = failedStatus, Value = failedStatus},
            new SelectListItem() { Text = archivedStatus, Value = archivedStatus}
        }, "Value", "Text");
    }


    class PrintUnPrintedDocumentModel  
    {
        [Key]
        public Int64 Id { get; set; }

        [StringLength(500)]
        public string EntityName { get; set; }

        [StringLength(128)]
        public string PrimaryKey { get; set; }

        [StringLength(4000)]
        public string FileLocation { get; set; }

        [StringLength(500)]
        public string PrinterName { get; set; }

        public DateTime? PrintedDateTime { get; set; }

        [StringLength(100)]
        public string Status { get; set; }
    }



    [Table("core.Printer")]
    public class PrinterModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }
        
        [ForeignKey("PrinterTray")]
        public int? PrinterTrayId { get; set; }
        public virtual PrinterTrayModel PrinterTray { get; set; }

        public bool IsActive { get; set; }
    }

    [Table("core.PrinterTray")]
    public class PrinterTrayModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int PrinterId { get; set; }
        
        public int Index { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
    
}