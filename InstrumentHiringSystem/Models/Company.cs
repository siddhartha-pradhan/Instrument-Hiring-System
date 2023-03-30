using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace InstrumentHiringSystem.Models
{
	public class Company
	{
		[Key]
		public int ID { get; set; }

		[Required]
		[Display(Name = "Company Name")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Street Address")]
		public string StreetAddress { get; set; }

		[Required]
		[Display(Name = "Residential City")]
		public string City { get; set; }

		[Required]
		[Display(Name = "State")]
		public string State { get; set; }

		[Required]
		[Display(Name = "Postal Code")]
		public int PostalCode { get; set; }

		[Required]
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
	}
}
