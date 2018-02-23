using System;
using System.Data.Entity;
using TFOF.Areas.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFOF.Areas.Email.Models
{
    public class EmailModelContext : BaseModelContext
	{
		public virtual DbSet<EmailInternalLogModel> EmailInternalLogs { get; set; }
		public virtual DbSet<EmailExternalLogModel> EmailExternalLogs { get; set; }
	}

	[Table("core.EmailInternalLog")]
	public class EmailInternalLogModel : BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(255)]
		[Display(Name = "Sent To")]
		public string SentTo { get; set; }

		[StringLength(255)]
		[Display(Name = "Sent CC")]
		public string SentCC { get; set; }

		[StringLength(255)]
		[Display(Name = "Sent BCC")]
		public string SentBCC { get; set; }

		[Required]
		[StringLength(255)]
		[Display(Name = "Subject")]
		public string Subject { get; set; }

		[Required]
		[Display(Name = "Body")]
		public string Body { get; set; }

		[Required]
		[Display(Name = "Sent Date")]
		public DateTime SentDate { get; set; }
	}

	[Table("core.EmailExternalLog")]
	public class EmailExternalLogModel : BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[Display(Name = "CustomerID")]
		public int CustomerID { get; set; }

		[Required]
		[StringLength(255)]
		[Display(Name = "Subject")]
		public string Subject { get; set; }

		[Required]
		[Display(Name = "Body")]
		public string Body { get; set; }

		[Required]
		[Display(Name = "Sent Date")]
		public DateTime SentDate { get; set; }

		[Required]
		[StringLength(255)]
		[Display(Name = "Sent To")]
		public string SentTo { get; set; }

		[StringLength(255)]
		[Display(Name = "Setn CC")]
		public string SentCC { get; set; }

		[StringLength(255)]
		[Display(Name = "Sent BCC")]
		public string SentBCC { get; set; }
	}
}