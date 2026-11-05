namespace Order.API.Dtos.OrderOfCustomer
{
    public class OrderDetailDto
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string OrderType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string ShippingAddress { get; set; }
        public decimal VoucherAmount { get; set; }
        public int PaymentMethod { get; set; } 
        public bool IsPaid { get; set; }
        public decimal ShippingFee { get; set; }

        public string? CancellationReason { get; set; }

        public List<OrderDetailItem> OrderDetails { get; set; }
        public string? BranchName { get; set; }
        public string? TableName { get; set; }
        

        public class OrderDetailItem
        {
            public string ProductName { get; set; }
            public string Img {get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }

        }
    }
}
