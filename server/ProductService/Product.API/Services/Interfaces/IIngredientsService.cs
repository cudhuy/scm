
using Product.API.Dtos.Ingredient;

namespace Product.API.Services.Interfaces
{
    public interface IIngredientsService
    {
        Task<bool> CreateIngredient(CreateIngredientDto createIngredientDto);
        Task<IEnumerable<IngredientDto>> GetAllIngredient();
    }
}
