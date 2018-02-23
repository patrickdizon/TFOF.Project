namespace TFOF.Areas.FieldOption.Models
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

    public class FieldOptionModelContext : BaseModelContext
    {
        public static SelectList GetByName(string name, bool useDescriptionAsText = false)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            BaseModelContext<FieldOptionModel> db = new BaseModelContext<FieldOptionModel>();
            var query = db.Models.Where(c => c.Name == name);
            return FieldOptionModelContext.MakeList(query.FirstOrDefault(), useDescriptionAsText);
        }

        public static SelectList GetBySlug(string slug, bool useDescriptionAsText = false)
        {   BaseModelContext<FieldOptionModel> db = new BaseModelContext<FieldOptionModel>();
            var query = db.Models.Where(c => c.Slug == slug);
            return FieldOptionModelContext.MakeList(query.FirstOrDefault(), useDescriptionAsText);
        }

        public static SelectList GetByRegistration(string modelField, bool useDescriptionAsText = false)
        {
            BaseModelContext<FieldOptionModelFieldModel> db = new BaseModelContext<FieldOptionModelFieldModel>();
            var query = db.Models.Where(c => c.ModelField == modelField);
            return FieldOptionModelContext.MakeList((query.FirstOrDefault() != null ? query.FirstOrDefault().FieldOption : null), useDescriptionAsText);
           
        }

        public static SelectList MakeList(FieldOptionModel fieldOptionModel, bool useDescriptionAsText = false)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (fieldOptionModel != null)
            {
                foreach (FieldOptionValueModel item in fieldOptionModel.FieldOptionValues.OrderBy(p => p.Position).ToList())
                {
                    var text = useDescriptionAsText ? item.Description : item.Value;
                    listItems.Add(new SelectListItem { Text = text, Value = item.Value });
                }
            }
            return new SelectList(listItems, "Value", "Text");
        }
    }

    /// <summary>
    /// Provides an override to SaveChanges in BaseModelContext
    /// </summary>
    /// <typeparam name="T">FieldOptiomValueModel</typeparam>
    public class FieldOptionModelContext<T> : BaseModelContext<T> where T : class
    {
        /// <summary>
        /// Increments Position property then calls base.SaveChanges()
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is FieldOptionValueModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach(var entry in entries)
            {
                if (((FieldOptionValueModel)entry.Entity).Position == 0)
                {
                    BaseModelContext<FieldOptionValueModel> db = new BaseModelContext<FieldOptionValueModel>();
                    int nextPosition = db.Models.Where(m => m.FieldOptionId == ((FieldOptionValueModel)entry.Entity).FieldOptionId).Count();
                    ((FieldOptionValueModel)entry.Entity).Position = nextPosition + 1;
                }
            }
            return base.SaveChanges();
        }
    }


    [Table("core.FieldOption")]
    public class FieldOptionModel: BaseModel
    {
        private string slug;

        [Key]
        public int Id { get; set; }

        [StringLength(150)]
        [Display(Name = "Name")]
        [Index("IX_FieldOption", 1, IsUnique = true)]
        public string Name { get; set; }
        
        [StringLength(150)]
        [Display(Name = "Slug")]
        [Index("IX_FieldOption1", 1, IsUnique = true)]
        public string Slug { 
            get
            {
                return this.slug;
            }

            set
            {
                if (value == null)
                {
                    this.slug = TFOF.Areas.Core.Services.TextService.Slugify(this.Name);
                } else
                {
                    this.slug = TFOF.Areas.Core.Services.TextService.Slugify(value);
                }
            }
        }

        public virtual ICollection<FieldOptionModelFieldModel> FieldOptionModelFields { get; set; }
        public virtual ICollection<FieldOptionValueModel> FieldOptionValues { get; set; }
    }


    [Table("core.FieldOptionModelField")]
    public class FieldOptionModelFieldModel : BaseModel
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Field Option")]
        [ForeignKey("FieldOption")]
        public int FieldOptionId { get; set; }

        [Display(Name = "Field Option")]
        public virtual FieldOptionModel FieldOption { get; set; }

        [StringLength(500)]
        [Display(Name = "Model Field")]
        [Index("IX_FieldOptionModelField", 1, IsUnique = true)]
        public string ModelField { get; set; }

       
    }


    [Table("core.FieldOptionValue")]
    public class FieldOptionValueModel: BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Field Option")]
        [ForeignKey("FieldOption")]
        public int FieldOptionId { get; set; }

        [Display(Name = "Field Option")]
        public virtual FieldOptionModel FieldOption { get; set; }

        /*Allows for specifying the order of items in a category.
        Use 'Position' instead of SQL reserve word 'Order'*/
        public int Position { get; set; }

        [StringLength(150)]
        public String Value { get; set; }

        [StringLength(500)]
        public String Description { get; set; }

        [DefaultValue("true")]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        
    }
}