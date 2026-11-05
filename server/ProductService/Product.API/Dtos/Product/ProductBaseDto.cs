namespace Product.API.Dtos.Product
{
    public class ProductBaseDto
    {
        public Guid Id { get; set; }
        //  public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string Name { get; set; }
       // public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public bool IsWishlist { get; set; } = false;

    }
    public class GetProductsBaseResponseDto
    {
        public List<ProductBaseDto> Items { get; set; } = new List<ProductBaseDto>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
