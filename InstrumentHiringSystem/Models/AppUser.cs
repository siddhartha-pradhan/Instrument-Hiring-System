using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace InstrumentHiringSystem.Models
{
	public class AppUser : IdentityUser
	{
		[Required]
		[Display(Name = "Full Name")]
		public string FullName { get; set; }

		[Display(Name = "City")]
		public string? CityAddress { get; set; }

		[Display(Name = "Region")]
		public string? RegionName { get; set; }

		public int? CompanyID { get; set; }

		[ForeignKey("CompanyID")]
		public Company? Company { get; set; }
	}
}
