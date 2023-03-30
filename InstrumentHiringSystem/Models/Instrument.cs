using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace InstrumentHiringSystem.Models
{
	public class Instrument : Base
	{
		[Key]
		public int ID { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		[Display(Name = "Standard Number")]
		public string StandardNumber { get; set; }

		[Required]
		public string Author { get; set; }

		[Display(Name = "List Price")]
		public double ListPrice { get; set; }

		public double Price { get; set; }

		[Display(Name = "Price for 20 Items")]
		public double Price20 { get; set; }

		[Display(Name = "Price for 50 Items")]
		public double Price50 { get; set; }

		[Display(Name = "Image URL")]
		public string? ImageURL { get; set; }

		[Display(Name = "Cover Type")]
		public int CoverTypeID { get; set; }

		[Display(Name = "Category")]
		public int CategoryID { get; set; }

		[ForeignKey("CategoryID")]
		public Category? Category { get; set; }

		[ForeignKey("CoverTypeID")]
		public CoverType? CoverType { get; set; }

		public bool IsDeleted { get; set; }
	}
}
