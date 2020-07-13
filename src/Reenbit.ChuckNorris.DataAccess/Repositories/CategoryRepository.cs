using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.Entities;


namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class CategoryRepository : EntityFrameworkCoreRepository<Category, int>, ICategoryRepository
    {
    }
}
