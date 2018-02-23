using PrintServiceCore.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServiceCore.Models
{
	/// <summary>
	/// A generic base model context that takes in a mapped EF model. Use the 'Models' (DbSet Class) property to query the list. 
	/// </summary>
	/// <typeparam name="T">EF Model</typeparam>
	public class BaseModelContext<T> : DbContext where T : class
	{
		public BaseModelContext() : base("name=BaseModelContext") { }

		public BaseModelContext(string modelContext) : base(modelContext) { }

		/// <summary>
		/// Used in conjunction with DecimalPrecisionAttribute, this method ensures that we save the appropriate amount of decimal places.
		/// Decimal by default is decimal(18,2).
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Properties().Where(x => x.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().Any())
					.Configure(c => c.HasPrecision(c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().First().Precision,
					c.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().First().Scale)
			);
		}

		public virtual DbSet<T> Models { get; set; }

		public virtual int SaveChanges(string userId)
		{
			var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
			foreach (var entry in entries)
			{
				DateTime datetime = DateTime.Now;
				((BaseModel)entry.Entity).Modified = datetime;
				((BaseModel)entry.Entity).ModifiedById = userId;
				if (entry.State == EntityState.Added)
				{
					((BaseModel)entry.Entity).Created = datetime;
					((BaseModel)entry.Entity).CreatedById = userId;

				}
				else
				{
					entry.Property("Created").IsModified = false;
					entry.Property("CreatedById").IsModified = false;
				}
			}
			int affected = base.SaveChanges();
			return affected;
		}
	}


	public class BaseModel 
	{
		[StringLength(128)]
		public string ModifiedById { get; set; }

		public DateTime? Modified { get; set; }

		[StringLength(128)]
		public string CreatedById { get; set; }

		public DateTime? Created { get; set; }

	}
}
