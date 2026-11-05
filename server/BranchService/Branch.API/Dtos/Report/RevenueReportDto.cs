namespace Branch.API.Dtos.Report
{
    public class RevenueReportDto
    {
        public decimal Total { get; set; }
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; }
        public string LeadingMonth { get; set; }

        public RevenueReportDto()
        {
            MonthlyRevenue = new List<MonthlyRevenueDto>();
        }
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public string CategoryName { get; set; }
        public string BranchName { get; set; }
    }

}
