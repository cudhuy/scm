namespace Product.API.Dtos.InventoryItem
{
    public class InventoryItemLowStockDto
    {
        //public Guid IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string BranchName { get; set; }
        public string IngredientQuantity { get; set; }
        public string ImportDate { get; set; }
        public string ExpiryDate { get; set; }
        public string AlertMessage { get; set; }
    }
}
