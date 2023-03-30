using InstrumentHiringSystem.Models;

namespace InstrumentHiringSystem.Repositories.Interfaces
{
    public interface IInstrumentRepository : IRepository<Instrument>
    {
        void Update(Instrument instrument);

        void Delete(Instrument instrument);
    }
}
