using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Product.API.Repositories.Interfaces;

namespace Product.API.Repositories.Behaviours
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ILogger<CategoryRepository> _logger;
        private readonly SCMDbContext _context;

        public CategoryRepository(ILogger<CategoryRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<List<Category>> GetListCategory()
        {
            _logger.LogInformation("Entered method GetListCategory in CategoryRepository");

            try
            {
                var categories = await _context.Categories
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} category.", categories.Count);
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category.");
                throw;
            }
        }
        public async Task<List<Category>> GetListCategoryUser()
        {
            _logger.LogInformation("Entered method GetListCategoryUser in CategoryRepository");

            try
            {
                var categories = await _context.Categories
                    .Where(v => !v.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} category.", categories.Count);
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category.");
                throw;
            }
        }
        public async Task<bool> CreateCategory(Category category)
        {
            _logger.LogInformation("Entered method CreateCategory in CategoryRepository");

            try
            {
                category.CreatedAt = DateTime.Now;
                category.UpdatedAt = DateTime.Now;
                category.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                category.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added new category with ID {Id}.", category.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the category.");
                return false;
            }
        }

        public async Task<bool> UpdateCategory(Guid id, Category updatedCategory)
        {
            _logger.LogInformation("Entered method UpdateCategory in 'CategoryRepository'");

            try
            {
                var category  = await _context.Categories.FirstOrDefaultAsync(v => v.Id == id);
                if (category == null)
                {
                    _logger.LogWarning("category with ID {Id} not found to update. 'CategoryRepository'", id);
                    return false;
                }

                category.Name = updatedCategory.Name;
                category.Description = updatedCategory.Description;

                category.UpdatedAt = DateTime.Now;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated category with ID {Id}.'CategoryRepository'", id);
                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the category.'CategoryRepository'");
                return false;
            }
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            return await _context.Categories.FirstOrDefaultAsync(v => v.Id == id);

        }
        public async Task<bool?> ToggleCategoryStatus(Guid id)
        {
            _logger.LogInformation("Entered ToggleCategoryStatus in CategoryRepository");

            try
            {
                var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found.", id);
                    return null;
                }

                category.IsDeleted = !category.IsDeleted;
                foreach (var product in category.Products)
                {
                    product.IsDeleted = category.IsDeleted;
                }
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Toggled IsDeleted status for category ID {Id} to {Status}", id, category.IsDeleted);
                return category.IsDeleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while toggling IsDeleted for category.");
                throw;
            }
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            _logger.LogInformation("Entered method DeleteCategory in 'CategoryRepository'");

            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(v => v.Id == id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found.'CategoryRepository'", id);
                    return false;
                }

                category.IsDeleted = true;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully marked category with ID {Id} as deleted. 'CategoryRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the category as deleted. 'CategoryRepository'");
                throw;
            }
        }
    }
}
