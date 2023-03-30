using Microsoft.AspNetCore.Mvc.Rendering;

namespace InstrumentHiringSystem.Models.ViewModels
{
	public class InstrumentViewModel
	{
		public Instrument Instrument { get; set; }

		public IEnumerable<SelectListItem>? CategoryList { get; set; }

		public IEnumerable<SelectListItem>? CoverTypeList { get; set; }
	}
}
