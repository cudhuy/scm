using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class LowStockAlert : BaseEntity
    {
        [Required]
        [ForeignKey("InventoryItem")]
        public Guid InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        [Required]
        [ForeignKey("Inventory")]
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        [Required]
        [ForeignKey("Ingredient")]
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        public string AlertMessage { get; set; }

        public DateTime AlertSentAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Trạng thái cảnh báo (0: Chưa xử lý, 1: Đã xử lý)
        /// </summary>
        public int Status { get; set; } = 0;
    }
}
