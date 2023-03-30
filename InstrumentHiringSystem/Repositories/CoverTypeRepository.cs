using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CoverTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(CoverType coverType)
        {
            coverType.IsDeleted = true;
            _dbContext.CoverTypes.Update(coverType);
        }

        public void Update(CoverType coverType)
        {
            coverType.LastModifiedAt = DateTime.Now;
            _dbContext.CoverTypes.Update(coverType);
        }
    }
}
