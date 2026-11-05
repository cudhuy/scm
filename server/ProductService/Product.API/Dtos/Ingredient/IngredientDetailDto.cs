namespace Product.API.Dtos.Ingredient
{
    public class IngredientDetailDto
    {
        public Guid IngredientId { get; set; }
        public string Name { get; set; }

        // Thông tin kho
        public string branchName { get; set; }
        public string Quantity { get; set; }
        public string LowStockThreshold { get; set; }

        // Thông tin các lô hàng
        public List<BatchDetailDto> BatchDetails { get; set; }

        // Thông tin nhà cung cấp
        public string Supplier { get; set; }
        public string AvgPrice { get; set; }
        public string TotalPrice { get; set; }

    }

    public class BatchDetailDto
    {
        public string Quantity { get; set; }
        public string ImportDate { get; set; }
        public string ExpiryDate { get; set; }
        public string UnitPrice { get; set; }
        public string Status { get; set; }
    }
}
