using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class ImageRepository : EntityFrameworkCoreRepository<ImageUrl, int>, IImageRepository
    {
    }
}
