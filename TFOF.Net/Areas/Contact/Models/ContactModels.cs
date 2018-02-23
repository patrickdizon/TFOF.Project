namespace TFOF.Areas.Contact.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using TFOF.Areas.Core.Models;

    using TFOF.Areas.FieldOption.Models;
    using System.Web.Mvc;




    [Table("dbo.Contact")]
    public class ContactModel  : BaseModel
    {
		[Key]
		public int Id { get; set; }

        [Required]
		[StringLength(255)]
		public string CompanyName { get; set; }

		[StringLength(50)]
		public string Category { get; set; }

        [Required]
        [StringLength(20)]
		public string MainPhoneNumber { get; set; }

		[StringLength(20)]
		public string MainFaxNumber { get; set; }

        [Required]
        [StringLength(150)]
		public string Address { get; set; }

        [Required]
        [StringLength(150)]
		public string City { get; set; }

        [Required]
        [StringLength(2)]
		public string State { get; set; }

        [Required]
        [StringLength(10)]
		public string ZipCode { get; set; }

		[StringLength(255)]
		public string Website { get; set; }

		[StringLength(50)]
		public string AccountNumber { get; set; }

		[StringLength(3)]
		public string Region { get; set; }

		[StringLength(50)]
		public string FirstName { get; set; }

		[StringLength(50)]
		public string LastName { get; set; }

		[StringLength(20)]
		public string PhoneNumber { get; set; }

		[StringLength(20)]
		public string PhoneNumberExtension { get; set; }

		[StringLength(20)]
		public string MobileNumber { get; set; }

		[StringLength(20)]
		public string AlternativePhoneNumber { get; set; }

		[StringLength(20)]
		public string FaxNumber { get; set; }

		[StringLength(100)]
		public string Email { get; set; }

		[StringLength(150)]
		public string BillingAddress { get; set; }

		[StringLength(100)]
		public string BillingCity { get; set; }

		[StringLength(2)]
		public string BillingState { get; set; }

		[StringLength(10)]
		public string BillingZipCode { get; set; }

		public string Notes { get; set; }


        public static SelectList categories = FieldOptionModelContext.GetByRegistration("ContactModel.Category");
        public static SelectList states = FieldOptionModelContext.GetByRegistration("ContactModel.State");

        //public override WellTableModel WellTable
        //{
        //    get { return new WellTableModel() { TableName = "[]", IdField = "Id" }; }
        //}
    }
}