using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ProductIngredient : BaseEntity
    {
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        [ForeignKey("Ingredient")]
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityRequired { get; set; }
        [MaxLength(50)]
        public string Unit { get; set; }


    }
}
