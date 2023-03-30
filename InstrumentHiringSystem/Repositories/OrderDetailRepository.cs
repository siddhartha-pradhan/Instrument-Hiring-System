using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
