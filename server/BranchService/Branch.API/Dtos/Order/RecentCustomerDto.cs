namespace Branch.API.Dtos.Order
{
    public class RecentCustomerDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastPurchase { get; set; }

    }
}
