using AutoMapper;
using Domain.Entities;
using Product.API.Dtos.Category;
using Product.API.Dtos.Ingredient;
using Product.API.Dtos.Product;

namespace Product.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Mapping category
            CreateMap<CreateCategoryDto, Category>().MaxDepth(64);
            CreateMap<UpdateCategoryDto, Category>().MaxDepth(64);

            // Mapping product
            CreateMap<Domain.Entities.Product, ProductDto>()
             .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty)).MaxDepth(64);
            CreateMap<Domain.Entities.Product, ProductDetailDto>()
              .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.ProductIngredients)).MaxDepth(64);
            CreateMap<ProductIngredient, IngredientOfProductDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Name : string.Empty)).MaxDepth(64);
               
                
            CreateMap<CreateProductDto, Domain.Entities.Product>().MaxDepth(64);
            CreateMap<UpdateProductDto, Domain.Entities.Product>().MaxDepth(64);

            // Mapping ingredient
            CreateMap<CreateIngredientDto, Ingredient>().MaxDepth(64);
        }
    }
}
