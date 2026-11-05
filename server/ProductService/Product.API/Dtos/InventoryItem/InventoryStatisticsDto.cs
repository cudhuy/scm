namespace Product.API.Dtos.InventoryItem
{
    public class InventoryStatisticsDto
    {
        public int TotalIngredients { get; set; } = 0;
        public int TotalWarnings { get; set; } = 0;
        public int TotalInventories { get; set; } = 0;
    }
}
