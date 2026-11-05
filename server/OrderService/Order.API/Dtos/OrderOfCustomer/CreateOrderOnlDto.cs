using System.ComponentModel.DataAnnotations;
using static Domain.Enums.OrderEnum;

namespace Order.API.Dtos.OrderOfCustomer
{
    public class CreateOrderOnlDto
    {
        public Guid CustomerId { get; set; }
        [Required, MaxLength(500)]
        public string ShippingAddress { get; set; } // Địa chỉ giao hàng

        [Required, MaxLength(20)]
        public string RecipientPhone { get; set; } // Số điện thoại người nhận

        [Required, MaxLength(100)]
        public string RecipientName { get; set; } // Tên người nhận hàng
        public Guid BranchId { get; set; }
        public decimal PreTotalAmount { get; set; }

        public decimal TotalAmount { get; set; }
        public List<OrderDetailDto>? OrderDetails { get; set; }
        public Guid? VoucherId { get; set; }
        public decimal VoucherAmount { get; set; }
        public decimal ShippingFee { get; set; }
        [Required]
        [EnumDataType(typeof(EPaymentMethod))]
        public int PaymentMethod { get; set; } // 1: Banking, 2: Cash


        public class OrderDetailDto
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            // public decimal Price { get; set; }
        }
    }
}
