namespace TFOF.Areas.Core.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using TFOF.Areas.Core.Attributes;

    public class AppModelContext : BaseModelContext
    {
       public virtual DbSet<AppModel> Apps { get; set; }
       public virtual DbSet<AppFieldModel> AppFields { get; set; }
    }

    /// <summary>
    /// Provides an override to SaveChanges in BaseModelContext
    /// </summary>
    /// <typeparam name="T">FieldOptiomValueModel</typeparam>
    public class AppFieldModelContext<T> : BaseModelContext<T> where T : class
    {
        /// <summary>
        /// Increments Position property then calls base.SaveChanges()
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is AppFieldModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (((AppFieldModel)entry.Entity).Position == 0)
                {
                    BaseModelContext<AppFieldModel> db = new BaseModelContext<AppFieldModel>();
                    int nextPosition = db.Models.Where(m => m.AppId == ((AppFieldModel)entry.Entity).AppId).Count();
                    ((AppFieldModel)entry.Entity).Position = nextPosition + 1;
                }
            }
            return base.SaveChanges();
        }
    }


    [Table("core.App")]
    public class AppModel : BaseModel
    {   
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Area { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(60)]
        public string NamePlural { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(120)]
        public string TitlePlural { get; set; }

        [Required]
        [StringLength(75)]
        public string TableName { get; set; }

        public bool InheritsBaseModel { get; set; }

        public virtual ICollection<AppFieldModel> AppFields { get; set; }
    }

    [Table("core.AppField")]
    public class AppFieldModel : BaseModel
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("App")]
        public int AppId { get; set; }
        public virtual AppModel App { get; set; }

        public int Position { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [StringLength(80)]
        public string DisplayName { get; set; }

        [StringLength(80)]
        public string ColumnName { get; set; }

        public bool IsPrimaryKey { get; set; }

        [StringLength(500)]
        public string ForeignKeyTo { get; set; }

        [StringLength(30)]
        public string DataType { get; set; }

        public int? StringLength { get; set; }

        [StringLength(150)]
        public string FieldOptionRegistration { get; set; }
            
        public bool IsSearchable { get; set; }

        public string DefaultValue { get; set; }

        public const string stringType = "string";
        public const string intType = "int";
        public const string intNullableType = "int?";
        public const string Int16Type = "Int16";
        public const string Int16NullableType = "Int16?";
        public const string Int64Type = "Int64";
        public const string Int64NullableType = "Int64?";
        public const string decimalType = "decimal";
        public const string decimalNullableType = "decimal?";
        public const string doubleType = "double";
        public const string doubleNullableType = "double?";
        public const string DateTimeType = "DateTime";
        public const string DateTimeNullableType = "DateTime?";
        public const string boolType = "bool";
        public const string guidType = "Guid";
        public const string guidNullableType = "Guid?";
        public const string DbGeographyType = "DbGeography";
        public const string DbGeographyNullableType = "DbGeography?";


        public static SelectList dataTypes = new SelectList(new List<SelectListItem>()
        {
            new SelectListItem() { Value = stringType, Text ="String"},
            new SelectListItem() { Value = intType, Text = "Integer" },
            new SelectListItem() { Value = intNullableType, Text = "Integer (nullable)" },
            new SelectListItem() { Value = Int16Type, Text = "Integer 16" },
            new SelectListItem() { Value = Int16NullableType, Text = "Integer 16 (nullable)" },
            new SelectListItem() { Value = Int64Type, Text = "Integer 64" },
            new SelectListItem() { Value = Int64NullableType, Text = "Integer 64 (nullable)" },
            new SelectListItem() { Value = decimalType, Text = "Decimal" },
            new SelectListItem() { Value = decimalNullableType, Text = "Decimal (nullable)" },
            new SelectListItem() { Value = doubleType, Text = "Double" },
            new SelectListItem() { Value = doubleNullableType, Text = "Double (nullable)" },
            new SelectListItem() { Value = DateTimeType, Text = "DateTime" },
            new SelectListItem() { Value = DateTimeNullableType, Text = "DateTime (nullable)" },
            new SelectListItem() { Value = boolType, Text ="Boolean"},
            new SelectListItem() { Value = guidType, Text ="Guid"},
            new SelectListItem() { Value = guidNullableType, Text ="Guid (Nullable)"},
            new SelectListItem() { Value = DbGeographyType, Text ="DbGeography"},
            new SelectListItem() { Value = DbGeographyNullableType, Text ="DbGeography (Nullable)"},


        }, "Value", "Text");
    }
}