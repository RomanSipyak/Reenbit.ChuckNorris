using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface IBaseRepository
    {
        void SetContext(DbContext context);
    }
}
