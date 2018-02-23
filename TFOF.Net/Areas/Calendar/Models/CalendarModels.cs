namespace TFOF.Areas.Calendar.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Core.Models;
    using System;

    [Table("dbo.Calendar")]
    public class CalendarModel  : BaseModel
    {
		[Key]
		public int Id { get; set; }

		public DateTime Date { get; set; }

		[StringLength(100)]
		public string Name { get; set; }

		public bool IsHoliday { get; set; }

		[StringLength(500)]
		public string Notes { get; set; }

		public bool IsClosedForBusiness { get; set; }
        
    }
}