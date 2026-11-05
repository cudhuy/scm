using Domain.Entities;

namespace Product.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetListCategory();
        Task<List<Category>> GetListCategoryUser();
        Task<bool> CreateCategory(Category category);
        Task<bool> UpdateCategory(Guid id, Category updatedCategory);
        Task<Category> GetCategoryById(Guid id);
        Task<bool> DeleteCategory(Guid id);
        Task<bool?> ToggleCategoryStatus(Guid id);

    }
}
