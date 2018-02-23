using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServiceCore.Models
{

    [Table("csw.Print")]
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

        public static string pdfPrinter = "PDF";
    }

    [Table("csw.PrintLog")]
    public class PrintLogModel : BaseModel
    {
        [Key]
        public Int64 Id { get; set; }

        public Int64 PrintId { get; set; }

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
    }

    [Table("csw.Printer")]
    public class PrinterModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [ForeignKey("PrinterTray")]
        public int PrinterTrayId { get; set; }
        public virtual PrinterTrayModel PrinterTray { get; set; }

        public bool IsActive { get; set; }
    }

    [Table("csw.PrinterTray")]
    public class PrinterTrayModel : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int PrinterId { get; set; }

        public int Index { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }

    [NotMapped]
    public class UnPrintedDocumentsModel
    {
      
        public long Id { get; set; }
        
        public string EntityName { get; set; }
        
        public string PrimaryKey { get; set; }
        
        public string FileLocation { get; set; }
      
        public string PrinterName { get; set; }

        public DateTime? PrintedDateTime { get; set; }
       
        public string Status { get; set; }
        public int RetryCount { get; set; }
    }
}
