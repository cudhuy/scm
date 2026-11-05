namespace Product.API.Dtos.Product
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        public string? Image { get; set; }
        public List<IngredientOfProductDto> Ingredients { get; set; } = new();

    }
    public class IngredientOfProductDto
    {
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; }
        public decimal QuantityRequired { get; set; }
        public string Unit { get; set; }
    }
}
