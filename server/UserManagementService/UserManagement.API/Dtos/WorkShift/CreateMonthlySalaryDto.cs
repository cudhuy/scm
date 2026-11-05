namespace UserManagement.API.Dtos.WorkShift
{
    public class CreateMonthlySalaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid EmployeeId { get; set; }
        public int TotalHour { get; set; }
        public decimal Bonus { get; set; }
        public decimal Penalty { get; set; }
        public decimal NetSalary { get; set; }
        public int TotalNumberOfShifts { get; set; }
        public int TotalLateShifts { get; set; }
        public int TotalAbsentShifts { get; set; }
        public int TotalNotCompleteShifts { get; set; }
        public decimal SalaryBase { get; set; }
        public bool IsConfirmed { get; set; } = true;
        public Guid BranchId { get; set; }
    }
}
