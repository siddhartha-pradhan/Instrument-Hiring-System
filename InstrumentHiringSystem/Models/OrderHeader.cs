using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstrumentHiringSystem.Models
{
	public class OrderHeader
	{
		[Key]
		public int ID { get; set; }

		public DateTime OrderedDate { get; set; }

		public DateTime ShippedDate { get; set; }

		public string UserID { get; set; }

		[ForeignKey("UserID")]
		public AppUser? ApplicationUser { get; set; }

		[Display(Name = "Order Total")]
		public double OrderTotal { get; set; }

		[Display(Name = "Order Status")]
		public string? OrderStatus { get; set; }

		[Display(Name = "Payment Status")]
		public string? PaymentStatus { get; set; }

		[Display(Name = "Tracking Number")]
		public string? TrackingNumber { get; set; }

		public string? Carrier { get; set; }

		[Display(Name = "Payment Date")]
		public DateTime PaymentDate { get; set; }

		[Display(Name = "Payment Due Date")]
		public DateTime PaymentDueDate { get; set; }

		[Display(Name = "Session")]
		public string? SessionID { get; set; }

		[Display(Name = "Payment Intent")]
		public string? PaymentIntentID { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }

		[Required]
		[Display(Name = "City Address")]
		public string City { get; set; }

		[Required]
		[Display(Name = "Region State")]
		public string Region { get; set; }
	}
}
