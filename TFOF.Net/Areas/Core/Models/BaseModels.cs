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
    using System.Web.Mvc;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Reflection;
    using Attributes;
    using System.Data.Entity.Validation;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;

    public class BaseModelContext : DbContext
	{
		// Your context has been configured to use a 'BaseModelContext' connection string from your application's 
		// configuration file (App.config or Web.config). By default, this connection string targets the 
		// 'TFOF.Areas.Core.Models.BaseModel' database on your LocalDb instance. 
		// 
		// If you wish to target a different database and/or database provider, modify the 'BaseModel' 
		// connection string in the application configuration file.
		public BaseModelContext() : base("name=BaseModelContext") {

			// Get the ObjectContext related to this DbContext
			var objectContext = (this as IObjectContextAdapter).ObjectContext;

			// Sets the command timeout for all the commands
			objectContext.CommandTimeout = 180;

		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Properties().Where(x => x.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().Any())
				.Configure(
					c => c.HasPrecision(
						c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().First().Precision,
						c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().First().Scale
					)
				);
		}
	   
		public override int SaveChanges()
		{
			string userId = HttpContext.Current.User.Identity.GetUserId();
			return SaveChanges(userId);
		}
		public int SaveChanges(string userId)
		{
			//Update Modified and ModifiedById For BaseModel classes 
			IEnumerable<DbEntityEntry> entries = ChangeTracker.Entries().Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
			foreach (DbEntityEntry entry in entries)
			{
				DateTime datetime = DateTime.Now;
				((BaseModel)entry.Entity).Modified = datetime;
				((BaseModel)entry.Entity).ModifiedById = userId;

				if (entry.State == EntityState.Added)
				{
					WellTableModel wellTableModel = ((BaseModel)entry.Entity).WellTable;
					if (wellTableModel != null && entry.Property(wellTableModel.IdField).CurrentValue == null)
					{
						entry.Property(wellTableModel.IdField).CurrentValue = ((BaseModel)entry.Entity).GetWellId();
					}
					((BaseModel)entry.Entity).Created = datetime;
					((BaseModel)entry.Entity).CreatedById = userId;

				}
				else
				{
					entry.Property("Created").IsModified = false;
					entry.Property("CreatedById").IsModified = false;
				}
			}

			//Update Id for BaseBaseModel classes
			IEnumerable<DbEntityEntry> idEntries = ChangeTracker.Entries().Where(x => x.Entity is BaseBaseModel && (x.State == EntityState.Added));
			foreach (DbEntityEntry entry in idEntries)
			{
				WellTableModel wellTableModel = ((BaseBaseModel)entry.Entity).WellTable;
				if (wellTableModel != null && entry.Property(wellTableModel.IdField).CurrentValue == null)
				{
					entry.Property(wellTableModel.IdField).CurrentValue = ((BaseBaseModel)entry.Entity).GetWellId();
				}
			}
			
			//Set IsModified = false for readonly Properties
			IEnumerable<DbEntityEntry> readOnlyEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Added ||x.State == EntityState.Modified);
			foreach (DbEntityEntry entry in readOnlyEntries)
			{
				var currentValues = entry.CurrentValues;
				if (currentValues != null)
				{
					foreach (string propertyName in currentValues.PropertyNames)
					{
						if (entry.Entity.GetType().GetProperty(propertyName).GetCustomAttribute(typeof(TFOF.Areas.Core.Attributes.ReadOnlyAttribute)) != null)
						{
							entry.Property(propertyName).IsModified = false;
						}
					}
				}
			}

			//Record Changes
			AuditModelContext recentUpdateDb = AuditChanges(ChangeTracker, userId);

			//Records need to be saved first before audit
			try
			{
				int affected = base.SaveChanges();
				recentUpdateDb.SaveChanges(userId);
				return affected;
			}
			catch (DbEntityValidationException ex)
			{
				foreach (var entryError in ex.EntityValidationErrors)
				{
					foreach (var error in entryError.ValidationErrors)
					{
						throw new DbEntityValidationException(
							"Entity Validation Failed for " + error.PropertyName + ": " +
							error.ErrorMessage
						); // Add the original exception as the inn
					}
				}

			}
			return 0;
		}

		/// <summary>
		/// Adds entries to the audit models and returns the RecentUpdateDb. As SaveChanges call is required to commit the changes from the caller.
		/// </summary>
		/// <param name="changeTracker"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public AuditModelContext AuditChanges(DbChangeTracker changeTracker, string userId)
		{
			//Record changes into the Audit DB.
			AuditModelContext recentUpdateDb = new AuditModelContext();

			//Create an exclusion list for audit
			string[] excludedFromAudit = new string[] { "ModifiedById", "Modified", "CreatedById", "Created" };
		  
			IEnumerable<DbEntityEntry> auditEntries = changeTracker.Entries().Where(x => x.Entity is BaseBaseModel && (x.State == EntityState.Modified));


			foreach (DbEntityEntry entry in auditEntries)
			{
                if (entry.GetType().GetCustomAttribute(typeof(NoAuditAttribute)) == null)
                {
                    var currentValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    if (currentValues != null)
                    {
                        foreach (string propertyName in currentValues.PropertyNames.Except(excludedFromAudit))
                        {
                            if (!string.IsNullOrWhiteSpace(propertyName))
                            {
                                var propertyValue = databaseValues.GetValue<object>(propertyName);

                                //Use nullPlaceholder for equals Operation.
                                object originalValue = propertyValue;
                                object currentValue = currentValues[propertyName];

                                //Audit changes and non-ReadOnly Fields.
                                if (entry.Entity.GetType().GetProperty(propertyName).GetCustomAttribute(typeof(TFOF.Areas.Core.Attributes.ReadOnlyAttribute)) == null)
                                {

                                    recentUpdateDb.Add(
                                        ObjectContext.GetObjectType(entry.Entity.GetType()).Name,
                                        ((BaseBaseModel)entry.Entity).GetPrimaryKey(),
                                        propertyName,
                                        originalValue,
                                        currentValue,
                                        userId
                                    );
                                }
                            }
                        }
                    }
                }
			}
			return recentUpdateDb;
		}

		
	}
	
	/// <summary>
	/// A generic base model context that takes in a mapped EF model. Use the 'Models' (DbSet Class) property to query the list. 
	/// </summary>
	/// <typeparam name="T">EF Model</typeparam>
	public class BaseModelContext<T> : BaseModelContext where T : class
	{
		/// <summary>
		/// Used in conjunction with DecimalPrecisionAttribute, this method ensures that we save the appropriate amount of decimal places.
		/// Decimal by default is decimal(18,2).
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Properties().Where(x => x.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().Any())
				.Configure(
					c => c.HasPrecision(
						c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().First().Precision,
						c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().First().Scale
					)
				);
			///Prevent creation of table for classes that are mapped to stored procedures.
			if(typeof(T).GetType().GetCustomAttribute(typeof(StoredProcedureAttribute)) != null)
			{
				modelBuilder.Ignore<T>();
			}
		}

		public virtual DbSet<T> Models { get; set; }
	}

	/// <summary>
	/// Will go away eventually but required during migration from paradox.
	/// </summary>
	public class BaseBaseModel
	{
		private WellTableModel wellTable;

		public virtual WellTableModel WellTable
		{
			get { return wellTable; }
			private set { wellTable = value; }
		}

		public string GetWellId(string tableName)
		{
			WellTable = new WellTableModel() { TableName = tableName };
			return GetWellId();
		}

		public string GetWellId()
		{
			if (WellTable != null)
			{
				SqlParameter param1 = new SqlParameter("@param1", this.WellTable.TableName);
				BaseModelContext<WellModel> db = new BaseModelContext<WellModel>();
				var query = db.Database.SqlQuery<WellModel>("GetWellID @param1", param1);
				foreach (var item in query)
				{
					return item.NewId;
				}
			}
			return null;
		}

		public string GetPrimaryKey()
		{
			foreach (PropertyInfo p in this.GetType().GetProperties())
			{
				Attribute ka = p.GetCustomAttribute(typeof(KeyAttribute));
				if (ka != null)
				{
					return p.GetValue(this).ToString();
				}
			}
			return null;
		}

		public static string Translation(IEnumerable<SelectListItem> itemList, string statusValue)
		{
			try
			{
				return itemList.Where(st => st.Value == statusValue).Single().Text;
			}
			catch
			{
				return null;
			}
		}

		public static string TranslationValue(IEnumerable<SelectListItem> itemList, string statusText)
		{
			try
			{
				return itemList.Where(st => st.Text == statusText).Single().Value; 
			}
			catch
			{
				return null;
			}
		}

	}
	/// <summary>
	/// BaseModel provides ModifiedById, Modified, CreatedById, Created and associated ModifiedBy and CreatedBy
	/// </summary>
	public class BaseModel : BaseBaseModel
	{   
		[StringLength(128)]
		[ForeignKey("ModifiedBy")]
		public string ModifiedById { get; set; }
		[Display(Name = "Modified By")]
		public virtual UserModel ModifiedBy { get; set; }

		public DateTime? Modified { get; set; }
		
		[StringLength(128)]
		[ForeignKey("CreatedBy")]
		public string CreatedById { get; set; }
		[Display(Name = "Created By")]
		public virtual UserModel CreatedBy { get; set; }

		public DateTime? Created { get; set; }

		public UserModel Modifier
		{
			get {
					if(this.ModifiedBy == null) {
					return new UserModel();
				}
				return this.ModifiedBy;
			}
		}

		public UserModel Creator
		{
			get {
				if(this.CreatedBy == null)
				{
					return new UserModel();
				}
				return this.CreatedBy;
			}
		}
        
	}

	[NotMapped]
	public class BaseDeleteModel
	{
		public string Url { get; set; }
		public string Id { get; set; }
		public string Title { get; set; }
        public string Message { get; set; }
        public bool Allowed { get; set; }

		public BaseDeleteModel(string url, string id , string title = "Confirm Deletion", string message = "Are you sure you want to delete this record? This cannot be undone.", bool allowed = true)
		{
			Url = url;
			Id = id;
			Title = title;
            Message = message;
            Allowed = allowed;
		}
	}
	
	[NotMapped]
	public class WellTableModel
	{
		public string TableName { get; set; }
		public string IdField { get; set; }
	}

	public class WellModel
	{
		[Key]
		public string NewId { get; set; }
	}
}


