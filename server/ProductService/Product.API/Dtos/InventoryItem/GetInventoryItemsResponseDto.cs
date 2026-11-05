namespace Product.API.Dtos.InventoryItem
{
    public class GetInventoryItemsResponseDto
    {
        public IEnumerable<InventoryItemDto> Items { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
