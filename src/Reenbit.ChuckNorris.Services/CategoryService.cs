using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public CategoryService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ICollection<string>> GetAllCategoriesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();

                return await categoryRepository.GetAllAsync(ct => ct.Title);
            }
        }
    }
}
