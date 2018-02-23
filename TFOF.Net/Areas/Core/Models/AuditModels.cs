namespace TFOF.Areas.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using Microsoft.AspNet.Identity;
    using User.Models;
    using System.Reflection;
    using System.Data.Entity.Core.Objects;

    public class AuditModelContext : DbContext
	{
		public AuditModelContext() : base("name=BaseModelContext") { }
		public DbSet<AuditModel> RecentUpdates { get; set; }

		public override int SaveChanges()
		{
			string userId = HttpContext.Current.User.Identity.GetUserId();
			return SaveChanges(userId);
		}

		public virtual int SaveChanges(string userId)
		{
			//var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));

			//foreach (var entry in entries)
			//{
			//	DateTime datetime = DateTime.Now;
			//	((BaseModel)entry.Entity).Modified = datetime;
			//	((BaseModel)entry.Entity).ModifiedById = userId;

			//	if (entry.State == EntityState.Added)
			//	{
			//		WellTableModel wellTableModel = ((BaseBaseModel)entry.Entity).WellTable;
			//		if (wellTableModel != null && entry.Property(wellTableModel.IdField).CurrentValue == null)
			//		{
			//			entry.Property(wellTableModel.IdField).CurrentValue = ((BaseBaseModel)entry.Entity).GetWellId();
			//		}
			//		((BaseModel)entry.Entity).Created = datetime;
			//		((BaseModel)entry.Entity).CreatedById = userId;

			//	}
			//	else
			//	{

			//		entry.Property("Created").IsModified = false;
			//		entry.Property("CreatedById").IsModified = false;
			//	}
			//}
			return base.SaveChanges();
		}

        /// <summary>
        /// This method adds an entry when the old property values are different from new property values.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public bool Add(string entityName, string primaryKey, string propertyName, object oldValue, object newValue, string createdById)
        {
            string nullPlaceholder = "[[[NULL]]]";
            string oldValueString = oldValue != null ? oldValue.ToString().Trim() : nullPlaceholder;
            string newValueString = newValue != null ? newValue.ToString().Trim() : nullPlaceholder;

            string[] ignoreValues = { "System.Collections.Generic", "System.Data.Entity", "TFOF.Areas" };

            foreach (string ignoreValue in ignoreValues)
            {
                if (newValueString.StartsWith(ignoreValue) || oldValueString.StartsWith(ignoreValue)) { return false; }
            }
            if (!newValueString.Equals(oldValueString))
            {
                RecentUpdates.Add(new AuditModel()
                {
                    EntityName = entityName,
                    PrimaryKey = primaryKey,
                    PropertyName = propertyName,
                    OldValue = oldValueString != nullPlaceholder ? oldValueString : null,
                    NewValue = newValueString != nullPlaceholder ? newValueString : null,
                    CreatedById = createdById
                });
                return true;
            }

            return false;
        }

        public AuditModelContext AuditChanges<T>(T oldModel, T newModel, string userId)
        {
            //Create an exclusion list for audit
            string[] excludedFromAudit = new string[] { "ModifiedById", "Modified", "CreatedById", "Created" };
            string primaryKey = null;


            MethodInfo primaryKeyMethod = oldModel.GetType().GetMethod("GetPrimaryKey");
            if (primaryKeyMethod != null)
            {
                primaryKey = primaryKeyMethod.Invoke(oldModel, null).ToString();
            }
            if (primaryKey != null)
            {
                foreach (PropertyInfo omp in oldModel.GetType().GetProperties())
                {
                    if (!excludedFromAudit.Contains(omp.Name))
                    {
                        //Use nullPlaceholder for equals Operation.
                        object originalValue = omp.GetValue(oldModel);
                        object currentValue = newModel.GetType().GetProperty(omp.Name).GetValue(newModel);

                        //Audit changes and non-ReadOnly Fields.
                        if (omp.GetCustomAttribute(typeof(TFOF.Areas.Core.Attributes.ReadOnlyAttribute)) == null)
                        {

                            Add(
                                ObjectContext.GetObjectType(oldModel.GetType()).Name,
                                primaryKey,
                                omp.Name,
                                originalValue,
                                currentValue,
                                userId
                            );

                        }

                    }

                }
            }
            return this;
        }

    }

	[Table("core._RecentUpdates")]
	public class AuditModel //Do not inherit BaseModels
	{
		[Key]
		public Int64 Id { get; set; }

		[StringLength(500)]
		public string EntityName { get; set; }
		
		[StringLength(500)]
		public string PropertyName { get; set; }
		
		[StringLength(128)]
		public string PrimaryKey { get; set; }

		public string OldValue { get; set; }

		public string NewValue { get; set; }
		
		[StringLength(128)]
		public string CreatedById { get; set; }
		public UserModel CreatedBy { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime? Created { get; set; }
	}
}


