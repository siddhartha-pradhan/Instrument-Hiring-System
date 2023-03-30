using System.ComponentModel.DataAnnotations;

namespace InstrumentHiringSystem.Models
{
	public class CoverType : Base
	{
		[Key]
		public int ID { get; set; }

		[Required]
		public string Title { get; set; }

		public bool IsDeleted { get; set; }
	}
}
