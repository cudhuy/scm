namespace Order.API.Dtos.OrderOfCustomer
{
    public class OrderOfACustomerDto
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public int PaymentMethod { get; set; } // 1: Banking, 2: Cash
        public bool IsPaid { get; set; } 
        public string OrderType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string ShippingAddress { get; set; }
        public decimal VoucherAmount { get; set; }
        public decimal ShippingFee { get; set; }

    }
    public class GetOrdersOfCustomerResponseDto
    {
        public List<OrderOfACustomerDto> Items { get; set; } = new List<OrderOfACustomerDto>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }


}
