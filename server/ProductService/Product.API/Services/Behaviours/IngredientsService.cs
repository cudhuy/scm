using AutoMapper;
using Domain.Entities;
using Product.API.Dtos.Ingredient;
using Product.API.Dtos.Product;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Interfaces;

namespace Product.API.Services.Behaviours
{
    public class IngredientsService : IIngredientsService
    {
        private readonly IIngredientsRepository _ingredientsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IngredientsService> _logger;

        public IngredientsService(IIngredientsRepository ingredientsRepository, IMapper mapper, ILogger<IngredientsService> logger)
        {
            _ingredientsRepository = ingredientsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CreateIngredient(CreateIngredientDto createIngredientDto)
        {
            try
            {
                _logger.LogInformation("Creating a new Ingredient. 'IngredientsService'");

                var ingredient = _mapper.Map<Ingredient>(createIngredientDto);
                var result = await _ingredientsRepository.CreateIngredient(ingredient);

                if (!result)
                {
                    _logger.LogWarning("Failed to create Ingredient. 'IngredientsService'");
                    return false;
                }

                _logger.LogInformation("Ingredient created successfully. 'IngredientsService'");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating ingredient. 'IngredientsService'");
                throw;
            }
        }

        public async Task<IEnumerable<IngredientDto>> GetAllIngredient()
        {
            try
            {
                var ingredients = await _ingredientsRepository.GetListIngredient();

                if (ingredients == null || !ingredients.Any())
                {
                    _logger.LogInformation("No ingredients found.");
                    return Enumerable.Empty<IngredientDto>();
                }

                var ingredientDtos = new List<IngredientDto>();

                foreach (var i in ingredients)
                {
                    var ingredientDto = new IngredientDto
                    {
                        IngredientId = i.Id ,
                        Name = i.Name,
                        Unit = i.Unit,

                    };
                    ingredientDtos.Add(ingredientDto);
                }

                _logger.LogInformation($"{ingredientDtos.Count} ingredientDtos found.");
                return ingredientDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllIngredient)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
