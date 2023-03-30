using InstrumentHiringSystem.Models;

namespace InstrumentHiringSystem.Repositories.Interfaces
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementCount(ShoppingCart cart, int count);

        int DecrementCount(ShoppingCart cart, int count);
    }
}
