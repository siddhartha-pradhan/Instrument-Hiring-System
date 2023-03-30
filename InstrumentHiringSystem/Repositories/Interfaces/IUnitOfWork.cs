namespace InstrumentHiringSystem.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        ICoverTypeRepository CoverType { get; }

        IInstrumentRepository Instrument { get; }

        ICompanyRepository Company { get; }

        IAppUserRepository AppUser { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IOrderHeaderRepository OrderHeader { get; }

        IOrderDetailRepository OrderDetail { get; }

        void Save();
    }
}
