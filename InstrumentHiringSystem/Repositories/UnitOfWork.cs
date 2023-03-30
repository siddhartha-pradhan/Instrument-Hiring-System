using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Category = new CategoryRepository(_dbContext);
            CoverType = new CoverTypeRepository(_dbContext);
            Instrument = new InstrumentRepository(_dbContext);
            Company = new CompanyRepository(_dbContext);
            AppUser = new AppUserRepository(_dbContext);
            ShoppingCart = new ShoppingCartRepository(_dbContext);
            OrderHeader = new OrderHeaderRepository(_dbContext);
            OrderDetail = new OrderDetailRepository(_dbContext);
        }

        public ICategoryRepository Category { get; set; }

        public ICoverTypeRepository CoverType { get; set; }

        public IInstrumentRepository Instrument { get; set; }

        public ICompanyRepository Company { get; set; }

        public IAppUserRepository AppUser { get; set; }

        public IShoppingCartRepository ShoppingCart { get; set; }

        public IOrderHeaderRepository OrderHeader { get; set; }

        public IOrderDetailRepository OrderDetail { get; set; }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
