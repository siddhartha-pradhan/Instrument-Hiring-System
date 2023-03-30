using InstrumentHiringSystem.Models;

namespace InstrumentHiringSystem.Repositories.Interfaces
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);

        void UpdateStatus(int ID, string orderStatus, string? paymentStatus = null);

        void UpdatePaymentStatus(int ID, string sessionID, string paymentIntentID);
    }
}
