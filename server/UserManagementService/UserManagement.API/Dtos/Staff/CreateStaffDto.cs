using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Dtos.Staff
{
    public class CreateStaffDto
    {
        public string? FullName { get; set; }

        public string Email { get; set; }

        [Required]
        public string Title { get; set; }

        public string Phone { get; set; }

        [Required]
        public int Type { get; set; } // Hourly or Fixed Salary

        [Required]
        public decimal SalaryBase { get; set; }

        [Required]
        public Guid BranchId { get; set; }

        public bool IsStaff { get; set; }
    }
}
