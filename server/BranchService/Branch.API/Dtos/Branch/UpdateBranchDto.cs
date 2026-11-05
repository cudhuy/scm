using System.ComponentModel.DataAnnotations;

namespace Branch.API.Dtos.Branch
{
    public class UpdateBranchDto
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

    }
}
