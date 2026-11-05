namespace Product.API.Dtos.InventoryItem
{
    public class InventoryItemWarningDto
    {
        public string IngredientName { get; set; }
        public bool IsStockLow { get; set; }
        public bool IsExpired { get; set; }
        public string WarningMessage { get; set; }
    }
}
