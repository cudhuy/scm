namespace Order.API.Dtos.OrderOfCustomer
{
    public class CreateOrderOffDto
    {
        public Guid? CustomerId { get; set; }
        public Guid TableId { get; set; }
        // public Guid BranchId { get; set; }
        public decimal PreTotalAmount { get; set; } // Tổng tiền hàng trước khi giảm giá
        public decimal TotalAmount { get; set; }
        public Guid? VoucherId { get; set; }
        public decimal VoucherAmount { get; set; }
       // public decimal ShippingFee { get; set; }


        public List<OrderDetailOffDtos>? OrderDetails { get; set; }

        public class OrderDetailOffDtos
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            //public decimal Price { get; set; }
        }
    }
}
