namespace Product.API.Dtos.Ingredient
{
    public class IngredientDto
    {
        public Guid IngredientId { get; set; }
        public string Name { get; set; }

        public string Unit { get; set; } // vd: ml, gram, gói, chai ...

    }
}
