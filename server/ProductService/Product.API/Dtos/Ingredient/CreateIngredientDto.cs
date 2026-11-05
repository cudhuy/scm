using System.ComponentModel.DataAnnotations;

namespace Product.API.Dtos.Ingredient
{
    public class CreateIngredientDto
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; } // vd: ml, gram, gói, chai ...

    }
}
