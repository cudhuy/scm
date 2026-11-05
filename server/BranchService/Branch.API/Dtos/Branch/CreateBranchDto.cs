using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Branch.API.Dtos.Branch
{
    public class CreateBranchDto
    {

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        //  [Required]
        //    public BranchEnum.BranchType Type { get; set; } // Sử dụng Enum

    }
}
