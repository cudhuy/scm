namespace Branch.API.Dtos.Order
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
