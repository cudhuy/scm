namespace Order.API.Dtos.OrderManagement
{
    public class OrderOffDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VoucherAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public int Status { get; set; }
        public int PaymentMethod { get; set; } // 1: Banking, 2: Cash
        public bool IsPaid { get; set; }
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
        public string OrderType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TableNumber { get; set; }
        public Guid TableId { get; set; }

    }

    public class GetOrdersResponseOffDto
    {
        public List<OrderOffDto> Items { get; set; } = new List<OrderOffDto>(); 
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public int CompletedOrdersCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}
