using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Ingredient : BaseEntity
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; } // vd: ml, gram, gói, chai ...

        public ICollection<ProductIngredient> ProductIngredients { get; set; }
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public ICollection<InventoryReceipt> InventoryReceipts { get; set; } = new List<InventoryReceipt>();
        public ICollection<LowStockAlert> LowStockAlerts { get; set; } = new List<LowStockAlert>();
    }
}
