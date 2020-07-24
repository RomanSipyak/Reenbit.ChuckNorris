using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface IImageRepository : IRepository<JokeImage, int>
    {
    }
}
