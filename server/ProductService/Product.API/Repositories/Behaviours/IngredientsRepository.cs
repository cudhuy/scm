using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product.API.Repositories.Interfaces;

namespace Product.API.Repositories.Behaviours
{
    public class IngredientsRepository : IIngredientsRepository
    {
        private readonly ILogger<IngredientsRepository> _logger;
        private readonly SCMDbContext _context;

        public IngredientsRepository(ILogger<IngredientsRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<bool> CreateIngredient(Ingredient ingredient)
        {
            _logger.LogInformation("Entered method CreateIngredient in IngredientsRepository");

            try
            {
                ingredient.CreatedAt = DateTime.Now;
                ingredient.UpdatedAt = DateTime.Now;
                ingredient.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                ingredient.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;

                await _context.Ingredients.AddAsync(ingredient);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added new ingredient with ID {Id}.", ingredient.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the ingredient.");
                return false;
            }
        }

        public async Task<List<Ingredient>> GetListIngredient()
        {
            _logger.LogInformation("Entered method GetListIngredient in IngredientsRepository");

            try
            {
                var ingredients = await _context.Ingredients
                    .Where(v => !v.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} Ingredients.", ingredients.Count);
                return ingredients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving ingredients.");
                throw;
            }
        }


    }
}
