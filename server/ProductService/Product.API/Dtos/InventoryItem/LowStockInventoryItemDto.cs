namespace Product.API.Dtos.InventoryItem
{
    public class LowStockInventoryItemDto
    {
        public string InventoryName { get; set; }
        public string IngredientName { get; set; }
        public Decimal RecentQuantity { get; set; }
        public Decimal AlertQuantity { get; set; }
        public string LastRestock { get; set; }
        public string ExpireDate { get; set; }
        public string AlertMessage { get; set; }
        public string CreateAt { get; set; }
    }
}
