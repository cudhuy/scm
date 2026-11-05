namespace Product.API.Dtos.InventoryItem
{
    public class InventoryItemDto
    {
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string IngredientQuantity { get; set; }
        public string Price { get; set; }
        public string BranchName { get; set; }
    }
}
