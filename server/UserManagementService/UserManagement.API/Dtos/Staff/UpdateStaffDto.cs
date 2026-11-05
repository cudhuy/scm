using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.API.Dtos.Staff
{
    public class UpdateStaffDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tittle phải từ 2 đến 100 ký tự.")]
        public string Title { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Type phải là 1 (Hourly) hoặc 2 (Fixed Salary).")]
        public int Type { get; set; } // 1 = Hourly hoặc 2 = Fixed Salary

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "SalaryBase phải là số dương.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalaryBase { get; set; }

        [Required]
        public Guid BranchId { get; set; }

        public bool IsActive { get; set; }


    }
    public class UpdateUserDto
    {
        [StringLength(255)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Dob { get; set; }
        public string Avatar { get; set; }
        [Column(TypeName = "enum('Active','Inactive')")]
        public string Status { get; set; }
    }
}
