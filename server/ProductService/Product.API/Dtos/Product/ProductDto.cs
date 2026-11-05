using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Product.API.Dtos.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }   
    }

    public class ProductResponse
    {
        public List<ProductDto> Items { get; set; } = new List<ProductDto>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}