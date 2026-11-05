using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Product.API.Dtos.Product
{
    public class CreateProductDto
    {
        public Guid CategoryId { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        // public int Status { get; set; } = 1;
        public IFormFile? Image { get; set; }
        [FromForm]
        public string Ingredients { get; set; }
    }
    public class ProductIngredientDTO
    {
        public Guid IngredientId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityRequired { get; set; }
        //[MaxLength(50)]
      //  public string Unit { get; set; }
    }
}
