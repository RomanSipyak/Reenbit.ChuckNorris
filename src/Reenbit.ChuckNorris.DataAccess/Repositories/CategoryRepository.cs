using Microsoft.EntityFrameworkCore;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class CategoryRepository : EntityFrameworkCoreRepository<Category, int>, ICategoryRepository
    {
        public Task<bool> CategoryExistAsync(Expression<Func<Category, bool>> predicate)
        {
            return this.DBSet.AnyAsync(predicate);
        }
    }
}
