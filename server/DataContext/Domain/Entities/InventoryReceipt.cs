using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class InventoryReceipt : BaseEntity
    {
        [Required]
        [ForeignKey("Inventory")]
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        [ForeignKey("Ingredient")]
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityReceived { get; set; } // Số lượng nhập

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Giá nhập hàng


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; } // Tổng chi phí

        public DateTime? ExpiryDate { get; set; } // Hạn sử dụng của lô hàng nhập
        public DateTime ReceivedAt { get; set; } = DateTime.Now; // Ngày nhập kho

        [MaxLength(255)]
        public string? Supplier { get; set; } // Nhà cung cấp (nếu có)

        public ICollection<InventoryItem> InventoryItems { get; set; }
    }
}
