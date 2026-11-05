using Domain.Entities;
using Product.API.Dtos.Category;

namespace Product.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategories();
        Task<bool> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<bool> UpdateCategory(Guid id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategory(Guid id);
        Task<IEnumerable<CategoryProductDto>> GetAllCategoryUser();
        Task<bool?> ToggleCategoryStatus(Guid id);

    }
}