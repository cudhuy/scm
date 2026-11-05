namespace Order.API.Dtos.OrderManagement
{
    public class TableDto
    {
        public Guid TableId { get; set; }
        public string TableNumber { get; set; }
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }

        public bool IsOccupied { get; set; }
        public int ActiveOrderCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
