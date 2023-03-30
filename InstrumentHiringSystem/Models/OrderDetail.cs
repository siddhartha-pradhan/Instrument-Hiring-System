using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstrumentHiringSystem.Models
{
	public class OrderDetail
	{
		[Key]
		public int ID { get; set; }

		public int OrderID { get; set; }

		public int InstrumentID { get; set; }

		public int Count { get; set; }

		public double Price { get; set; }

		[ForeignKey("OrderID")]
		public OrderHeader? OrderHeader { get; set; }

		[ForeignKey("InstrumentID")]
		public Instrument? Instrument { get; set; }
	}
}
