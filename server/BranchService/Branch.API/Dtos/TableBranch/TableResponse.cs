namespace Branch.API.Dtos.TableBranch
{
    public class TableResponse
    {
        public List<TableDto> Items { get; set; } = new List<TableDto>();
        public int? NextPage { get; set; }
    }
    public class TableDto
    {
        public Guid Id { get; set; }
        public String TableNumber { get; set; }
        public Guid BranchId { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsDeleted { get; set; }
    }
}
