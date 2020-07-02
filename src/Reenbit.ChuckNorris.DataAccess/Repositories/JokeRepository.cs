using Microsoft.EntityFrameworkCore;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class JokeRepository : EntityFrameworkCoreRepository<Joke, int>, IJokeRepository
    {
        public async Task<Joke> GetByIdWithCategoryIncludeAsync(int id)
        {
            return await this.DBSet.Where(jk => jk.Id == id)
                              .Include(jk => jk.JokeCategories)
                              .ThenInclude(jk => jk.Category)
                              .FirstOrDefaultAsync();
        }
    }
}
