using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(Company company)
        {
            _dbContext.Companies.Update(company);
        }

        public void Update(Company company)
        {
            _dbContext.Companies.Update(company);
        }
    }
}
