using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class InstrumentRepository : Repository<Instrument>, IInstrumentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public InstrumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(Instrument instrument)
        {
            instrument.IsDeleted = true;
            _dbContext.Instruments.Update(instrument);
        }

        public void Update(Instrument instrument)
        {
            instrument.LastModifiedAt = DateTime.Now;
            _dbContext.Instruments.Update(instrument);
        }
    }
}
