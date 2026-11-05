using System.ComponentModel.DataAnnotations;

namespace Product.API.Dtos.InventoryItem
{
    public class RestockInventoryDto
    {

        //[Required(ErrorMessage = "InventoryId is required.")]
        //public Guid InventoryId { get; set; }

        [Required(ErrorMessage = "BranchId is required.")]
        public Guid BranchId { get; set; }

        [Required(ErrorMessage = "IngredientId is required.")]
        public Guid IngredientId { get; set; }

        [Required(ErrorMessage = "IngredientName is required.")]
        [StringLength(100, ErrorMessage = "IngredientName must be at most 100 characters.")]
        public string IngredientName { get; set; }

        [Required(ErrorMessage = "IngredientUnit is required.")]
        [StringLength(50, ErrorMessage = "IngredientUnit must be at most 50 characters.")]
        public string IngredientUnit { get; set; }

        [Required(ErrorMessage = "QuantityReceived is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "QuantityReceived must be greater than 0.")]
        public decimal QuantityReceived { get; set; }

        [Required(ErrorMessage = "LowStockThreshold is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "LowStockThreshold must be a positive number.")]
        public decimal LowStockThreshold { get; set; }

        [Required(ErrorMessage = "UnitPrice is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "UnitPrice must be a positive number.")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "ExpiryDate is required.")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "SupplierName is required.")]
        [StringLength(100, ErrorMessage = "SupplierName must be at most 100 characters.")]
        public string SupplierName { get; set; }

    }
}
