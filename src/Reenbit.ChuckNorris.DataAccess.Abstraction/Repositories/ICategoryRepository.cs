using Reenbit.ChuckNorris.Domain.Entities;
using System.Collections.Generic;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface ICategoryRepository : IRepository<Category, int>
    {
        void RemoveLinkedJokeCategories(ICollection<JokeCategory> jokeCategories);
    }
}
