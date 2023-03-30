using InstrumentHiringSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstrumentHiringSystem.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
		{

		}

		public DbSet<Category> Categories { get; set; }

		public DbSet<CoverType> CoverTypes { get; set; }

		public DbSet<Instrument> Instruments { get; set; }

		public DbSet<Company> Companies { get; set; }

		public DbSet<ShoppingCart> ShoppingCarts { get; set; }

		public DbSet<AppUser> AppUsers { get; set; }

		public DbSet<OrderHeader> OrderHeaders { get; set; }

		public DbSet<OrderDetail> OrderDetails { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<IdentityUser>().ToTable("Users");
			builder.Entity<IdentityRole>().ToTable("Roles");
			builder.Entity<IdentityUserToken<string>>().ToTable("Tokens");
			builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
			builder.Entity<IdentityUserLogin<string>>().ToTable("LoginAttempts");
			builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
			builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
		}
	}
}
