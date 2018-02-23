


namespace TFOF.Areas.Core.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Web.Mvc;

    [Table("core.Menu")]
    public class MenuModel : BaseModel
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        public virtual MenuModel Parent { get; set; }

        [StringLength(150)]
        public string Label { get; set; }

        public int GroupNumber { get; set; } = 1;
    
        public int Position { get; set; }

        [StringLength(50)]
        public string Icon { get; set; }
        
        [StringLength(255)]
        public string Role { get; set; }

        [StringLength(100)]
        public string Action { get; set; } = "Index";

        [StringLength(100)]
        public string Area { get; set; }

        [StringLength(100)]
        public string Controller { get; set; }

        [StringLength(20)]
        public string Environment { get; set; }

        public virtual ICollection<MenuModel> SubMenus { get; set; }

        public const string devEnvironment  = "DEV";
        public const string prodEnvironment = "PROD";
        public const string uatEnvironment = "UAT";

        public static SelectList environments = new SelectList(new List<SelectListItem>()
        {
            new SelectListItem() { Value = devEnvironment, Text = "Development" },
            new SelectListItem() { Value = uatEnvironment, Text = "User Acceptance Testing" },
            new SelectListItem() { Value = prodEnvironment, Text = "Production" },

        }, "Value", "Text");
    }
   
}