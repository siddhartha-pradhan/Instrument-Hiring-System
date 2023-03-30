using InstrumentHiringSystem.Models;

namespace InstrumentHiringSystem.Repositories.Interfaces
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        void Update(CoverType coverType);

        void Delete(CoverType coverType);
    }
}
