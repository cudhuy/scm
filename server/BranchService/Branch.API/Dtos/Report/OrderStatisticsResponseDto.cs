namespace Branch.API.Dtos.Report
{
    public class OrderStatisticsResponseDto
    {
        public Guid? BranchId { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<OrderData> OrderStatisticsData { get; set; }
        public int HighestOrderInWeek { get; set; }
    }

    public class OrderData
    {
        public string? Day { get; set; }
        public int Count { get; set; }
    }
}
