using Microsoft.AspNetCore.Routing.Template;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface ICategoryRepository : IRepository<Category, int>
    {
        void RemoveCategory(Category category);
    }
}
