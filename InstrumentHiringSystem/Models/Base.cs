namespace InstrumentHiringSystem.Models
{
	public class Base
	{
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public DateTime? LastModifiedAt { get; set; }
	}
}
