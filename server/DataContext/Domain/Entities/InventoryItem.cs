using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class InventoryItem : BaseEntity
    {
        [ForeignKey("Inventory")]
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        [ForeignKey("Ingredient")]
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LowStockThreshold { get; set; } = 10;
        public DateTime LastRestocked { get; set; } = DateTime.Now;

        public DateTime ExpiryDate { get; set; } = DateTime.Now;

        [ForeignKey("InventoryReceipt")]
        public Guid? InventoryReceiptId { get; set; }
        public InventoryReceipt? InventoryReceipt { get; set; }

        public ICollection<LowStockAlert> LowStockAlerts { get; set; } = new List<LowStockAlert>();
    }
}
