using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.Entities;
using System.Linq;

namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class CategoryRepository : EntityFrameworkCoreRepository<Category, int>, ICategoryRepository
    {
        public void RemoveCategory(Category category)
        {
            if (category != null)
            {
                RemoveLinkedJokeCategories(category);
                this.Remove(category);
            }
        }

        private void RemoveLinkedJokeCategories(Category category)
        {
            var jokeCategories = this.DbContext.Set<JokeCategory>().AsQueryable().Where(jc => jc.CategoryId == category.Id);
            if (jokeCategories.Count() != 0)
            {
                this.DbContext.Set<JokeCategory>().RemoveRange(jokeCategories);
            }
        }
    }
}
