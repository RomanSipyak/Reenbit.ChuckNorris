using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface IJokeRepository : IRepository<Joke, int>
    {
    }
}
