namespace Product.API.Dtos.InventoryItem
{
    public class InventoryReceiptDto
    {
        public string CreateAt { get; set; }
        public string IngredientName { get; set; }
        public string IngredientQuantity { get; set; }
        public string BranchName { get; set; }
        public string UnitPrice { get; set; }
        public string TotalPrice { get; set; }
        public string InventoryReceiver { get; set; }
    }
}
