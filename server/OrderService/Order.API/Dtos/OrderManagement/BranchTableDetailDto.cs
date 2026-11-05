namespace Order.API.Dtos.OrderManagement
{
    public class BranchTableDetailDto
    {
        public string TableNumber { get; set; }
        public bool IsOccupied { get; set; }
        public List<OrderOfTableDto> Orders { get; set; }
    }
    public class OrderOfTableDto
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public int Status { get; set; }
        public decimal Type { get; set; } 
        public decimal VoucherAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderOffProductDto> Products { get; set; }
    }
    
    public class OrderOffProductDto
    {
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
     //   public decimal TotalAmount { get; set; }
    }
}
