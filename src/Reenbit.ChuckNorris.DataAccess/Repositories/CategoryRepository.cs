using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class CategoryRepository : EntityFrameworkCoreRepository<Category, int>, ICategoryRepository
    {
        public void RemoveCategory(Category category)
        {
            this.Remove(category);
        }

        public void RemoveLinkedJokeCategories(ICollection<JokeCategory> jokeCategories)
        {
            this.DbContext.Set<JokeCategory>().RemoveRange(jokeCategories);
        }
    }
}
