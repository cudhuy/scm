namespace Branch.API.Dtos.TableBranch
{
    public class TableNotDeletedResponse
    {
        public List<TableNotDeletedDto> Items { get; set; } = new List<TableNotDeletedDto>();
        public int? NextPage { get; set; }
    }
    public class TableNotDeletedDto
    {
        public Guid Id { get; set; }
        public String TableNumber { get; set; }
        public Guid BranchId { get; set; }
        public bool IsOccupied { get; set; }
    }
}
