using AutoMapper;
using Domain.Entities;
using Product.API.Dtos.Category;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Interfaces;

namespace Product.API.Services.Behaviours
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetListCategory();

                if (categories == null || !categories.Any())
                {
                    _logger.LogInformation("No categories found.");
                    return Enumerable.Empty<CategoryDto>();
                }

                var categoryDtos = new List<CategoryDto>();

                foreach (var c in categories)
                {
                    var categoryDto = new CategoryDto
                    {
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        CreatedBy = c.CreatedBy,
                        IsDeleted = c.IsDeleted
                    };
                    categoryDtos.Add(categoryDto);
                }

                _logger.LogInformation($"{categoryDtos.Count} categoryDto found.");
                return categoryDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllCategories)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        public async Task<IEnumerable<CategoryProductDto>> GetAllCategoryUser()
        {
            try
            {
                var categories = await _categoryRepository.GetListCategoryUser();

                if (categories == null || !categories.Any())
                {
                    _logger.LogInformation("No categories found.");
                    return Enumerable.Empty<CategoryProductDto>();
                }

                var categoryDtos = new List<CategoryProductDto>();

                foreach (var c in categories)
                {
                    var categoryDto = new CategoryProductDto
                    {
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        CreatedBy = c.CreatedBy
                    };
                    categoryDtos.Add(categoryDto);
                }

                _logger.LogInformation($"{categoryDtos.Count} categoryDto found.");
                return categoryDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllCategories)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        public async Task<bool> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            try
            {
                _logger.LogInformation("Creating a new category. 'CategoryService'");

                var category = _mapper.Map<Category>(createCategoryDto);
                var result = await _categoryRepository.CreateCategory(category);

                if (!result)
                {
                    _logger.LogWarning("Failed to create category. 'CategoryService'");
                    return false;
                }

                _logger.LogInformation("Category created successfully. 'CategoryService'");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating category. 'CategoryService'");
                throw;
            }
        }

        public async Task<bool> UpdateCategory(Guid id, UpdateCategoryDto updateCategoryDto)
        {
            _logger.LogInformation("Entered UpdateUserRole  in service for role with id {id}.'CategoryService'", id);

            try
            {
                var existingCate = await _categoryRepository.GetCategoryById(id);
                if (existingCate == null)
                {
                    _logger.LogWarning("Category with id {id} not found.'CategoryService'", id);
                    return false;
                }


                var updatedCate= _mapper.Map<Category>(updateCategoryDto);
                // updatedCategory.Id = existingCategory.Id;

                var resultCategoryUpdate = await _categoryRepository.UpdateCategory(id, updatedCate);

                if (resultCategoryUpdate == null)
                {
                    _logger.LogWarning("Failed to update Category with id {Id}.'CategoryService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully updated Category with id {Id}.'CategoryService'", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(UpdateCategory)}: {ex.Message}, StackTrace: {ex.StackTrace}.'CategoryService'");
                throw;
            }
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            _logger.LogInformation("Entered method DeleteCategory in service for category ID {Id}'CategoryService'", id);

            try
            {
                var result = await _categoryRepository.DeleteCategory(id);
                if (!result)
                {
                    _logger.LogWarning("Category with ID {Id} not found in service.'CategoryService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully marked category with ID {Id} as deleted in service.'CategoryService'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(DeleteCategory)}: {ex.Message}, StackTrace: {ex.StackTrace}'CategoryService'");
                throw;
            }
        }

        public async Task<bool?> ToggleCategoryStatus(Guid id)
        {
            _logger.LogInformation("Entered ToggleCategoryStatus in CategoryService for ID {Id}", id);

            try
            {
                var result = await _categoryRepository.ToggleCategoryStatus(id);
                if (result == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found in service.", id);
                    return null;
                }

                _logger.LogInformation("Successfully toggled IsDeleted for category ID {Id} to {Status}", id, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ToggleCategoryStatus: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }



}
