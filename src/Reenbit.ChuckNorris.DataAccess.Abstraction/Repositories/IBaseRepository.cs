using Microsoft.EntityFrameworkCore;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface IBaseRepository
    {
        void SetContext(DbContext context);
    }
}
