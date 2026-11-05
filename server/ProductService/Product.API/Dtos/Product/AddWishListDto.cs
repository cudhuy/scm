namespace Product.API.Dtos.Product
{
    public class AddWishListDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
