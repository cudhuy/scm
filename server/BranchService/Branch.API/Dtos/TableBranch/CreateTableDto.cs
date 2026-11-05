using System.ComponentModel.DataAnnotations;

namespace Branch.API.Dtos.TableBranch
{
    public class CreateTableDto
    {
        [Required]
        public Guid BranchId { get; set; } 

        [Required]
        [StringLength(50)]
        public string TableNumber { get; set; } 
    }
}
