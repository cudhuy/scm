using System.ComponentModel.DataAnnotations;

namespace Product.API.Dtos.Category
{
    public class UpdateCategoryDto
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
