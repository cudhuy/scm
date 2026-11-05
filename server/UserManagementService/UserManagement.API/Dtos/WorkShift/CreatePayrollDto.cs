namespace UserManagement.API.Dtos.WorkShift
{
    public class CreatePayrollDto
    {
        public Guid BrandId { get; set; }
        public DateOnly PayrollDate { get; set; }
    }
}
