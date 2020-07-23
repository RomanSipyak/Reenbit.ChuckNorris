using Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<ICollection<CategoryDto>> GetAllCategoriesAsync();

        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDTO);

        Task<bool> CategoryExistsAsync(string categoryTitle);

        Task DeleteCategoryAsync(int categoryId);
    }
}
