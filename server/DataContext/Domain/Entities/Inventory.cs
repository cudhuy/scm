using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Inventory : BaseEntity
    {

        [ForeignKey("Branches")]
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int Status { get; set; } = 1;
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public ICollection<InventoryReceipt> InventoryReceipts { get; set; } = new List<InventoryReceipt>();
        public ICollection<LowStockAlert> LowStockAlerts { get; set; } = new List<LowStockAlert>();
    }
}
