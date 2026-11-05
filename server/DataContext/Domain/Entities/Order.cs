using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        [Required, MaxLength(50)]
        public string OrderCode { get; set; }

        [ForeignKey("User")]
        public Guid? CustomerId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("Branch")]
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        [ForeignKey("BranchTable")]
        public Guid? TableId { get; set; }
        public BranchTable? Table { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public int Status { get; set; } = 0;

        [Required, MaxLength(500)]
        public string ShippingAddress { get; set; } // Địa chỉ giao hàng

        [Required, MaxLength(20)]
        public string RecipientPhone { get; set; } // Số điện thoại người nhận

        [Required, MaxLength(100)]
        public string RecipientName { get; set; } // Tên người nhận hàng

        [Required, MaxLength(50)]
        public string OrderType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0; //phí ship

        [Column(TypeName = "decimal(18,2)")]
        public decimal VoucherAmount { get; set; } = 0; //tiền voucher

        // Thêm trường PaymentMethod (1: Banking, 2: Cash)
        public int PaymentMethod { get; set; } // 1: Banking, 2: Cash

        // Thêm trường IsPaid (true: Đã thanh toán, false: Chưa thanh toán)
        public bool IsPaid { get; set; } = false; // Mặc định là chưa thanh toán

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public List<Payment> Payments { get; set; } = new List<Payment>();

        public Guid voucherId { get; set; }
        public Feedback? Feedback { get; set; }
    }
}
