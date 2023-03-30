using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(OrderHeader orderHead)
        {
            _dbContext.OrderHeaders.Update(orderHead);
        }

        public void UpdateStatus(int ID, string orderStatus, string? paymentStatus)
        {
            var order = _dbContext.OrderHeaders.FirstOrDefault(u => u.ID == ID);

            if (order != null)
            {
                order.OrderStatus = orderStatus;

                if (paymentStatus != null)
                {
                    order.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdatePaymentStatus(int ID, string sessionID, string paymentIntentID)
        {
            var order = _dbContext.OrderHeaders.FirstOrDefault(u => u.ID == ID);

            if (order != null)
            {
                order.PaymentDate = DateTime.Now;
                order.SessionID = sessionID;
                order.PaymentIntentID = paymentIntentID;
            }
        }
    }
}
