using Domain.Entities;

namespace Product.API.Repositories.Interfaces
{
    public interface IIngredientsRepository
    {
        Task<bool> CreateIngredient(Ingredient ingredient);
        Task<List<Ingredient>> GetListIngredient();
    }
}
