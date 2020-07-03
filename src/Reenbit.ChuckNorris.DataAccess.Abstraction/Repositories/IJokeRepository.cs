using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface IJokeRepository : IRepository<Joke, int>
    {
        public Task<JokeDTO> GetJokeDtoByIdWithCategoriesAsync(int id);
    }
}
