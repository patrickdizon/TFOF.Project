namespace TFOF.Areas.PDF.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using TFOF.Areas.Core.Models;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel;
    using System.Web.Mvc;
    using System.Collections.Generic;
    
    public class PDFModelContext : BaseModelContext
    {
        public virtual DbSet<PDFModel> PDFs { get; set; }
    }
    

    [Table("core.PDF")]
    public class PDFModel: BaseModel
    {
        public int Id { get; set; }
        
        public int CustomerId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string HtmlContent { get; set; }
        

    }
}