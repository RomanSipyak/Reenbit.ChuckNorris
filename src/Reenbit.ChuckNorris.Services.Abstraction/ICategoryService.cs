using Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<ICollection<CategoryDTO>> GetAllCategoriesAsync();

        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO);

        Task<bool> CategoryExistsAsync(string categoryTitle);

        Task DeleteCategoryAsync(int categoryId);
    }
}
