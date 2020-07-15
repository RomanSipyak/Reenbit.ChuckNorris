using AutoMapper;
using AutoMapper.QueryableExtensions.Impl;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IMapper mapper;

        public CategoryService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.mapper = mapper;
        }

        public async Task<ICollection<string>> GetAllCategoriesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                return await categoryRepository.FindAndMapAsync(c => c.Title);
            }
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                if (await categoryRepository.AnyAsync(c => c.Title == categoryDTO.Title))
                {
                    throw new ArgumentException($"Category with {categoryDTO.Title} already exist");
                }

                Category category = mapper.Map<Category>(categoryDTO);
                categoryRepository.Add(category);
                await uow.SaveChangesAsync();
                var categoryDto = mapper.Map<CategoryDTO>(category);
                return categoryDto;
            }
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                var category = (await categoryRepository.FindAndMapAsync(c => c,
                                                             c => c.Id == categoryId,
                                                             null,
                                                             new List<Expression<Func<Category, object>>> { j => j.JokeCategories }))
                                                             .FirstOrDefault();
                if (category != null)
                {
                    if (category.JokeCategories.Count != 0)
                    {
                        categoryRepository.RemoveLinkedJokeCategories(category.JokeCategories);
                    }

                    categoryRepository.Remove(category);
                }

                await uow.SaveChangesAsync();
            }
        }
    }
}
