using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InstrumentHiringSystem.Models
{
	public class ShoppingCart
	{
		[Key]
		public int ID { get; set; }

		public string UserID { get; set; }

		public int InstrumentID { get; set; }

		[Range(1, 1000, ErrorMessage = "Enter a value from 1 to 1000")]
		public int Count { get; set; }

		[NotMapped]
		public double Price { get; set; }

		[ForeignKey("InstrumentID")]
		public Instrument? Instrument { get; set; }

		[ForeignKey("UserID")]
		public AppUser? AppUser { get; set; }

		
	}
}
