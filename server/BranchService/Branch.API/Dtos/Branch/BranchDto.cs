using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Branch.API.Dtos.Branch
{
    public class BranchDto

    {
        public Guid Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [Column(TypeName = "enum('Main','Sub')")]
        public string Type { get; set; }
    }
}
