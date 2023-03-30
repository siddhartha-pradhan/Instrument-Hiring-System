using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AppUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
