namespace UserManagement.API.Dtos.WorkShift
{
    public class GetSalaryDto
    {
        public Guid BranchId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
