namespace Product.API.Dtos.Product
{
    public class ProductDetailUserDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }

    }
}
