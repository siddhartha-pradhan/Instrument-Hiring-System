using InstrumentHiringSystem.Data;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;

namespace InstrumentHiringSystem.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Category category)
        {
            category.LastModifiedAt = DateTime.Now;
            _dbContext.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            category.IsDeleted = true;
            _dbContext.Categories.Update(category);
        }
    }
}
