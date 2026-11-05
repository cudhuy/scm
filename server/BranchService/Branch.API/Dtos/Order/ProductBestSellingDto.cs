namespace Branch.API.Dtos.Order
{
    public class ProductBestSellingDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string Image { get; set; }

    }
}
