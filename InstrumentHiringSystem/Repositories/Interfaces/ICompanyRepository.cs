using InstrumentHiringSystem.Models;

namespace InstrumentHiringSystem.Repositories.Interfaces
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);

        void Delete(Company company);
    }
}
