namespace UserManagement.API.Dtos.WorkShift
{
    public class CreateEmployeeShiftDto
    {
        public Guid WorkShiftId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
        public Guid BranchId { get; set; }
    }
}
