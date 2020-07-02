using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        T GetRepository<T>() where T : class, IBaseRepository;
    }
}
