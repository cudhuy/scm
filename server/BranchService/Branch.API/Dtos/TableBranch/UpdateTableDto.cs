using System.ComponentModel.DataAnnotations;

namespace Branch.API.Dtos.TableBranch
{
    public class UpdateTableDto
    {
        [Required]
        [StringLength(50)]
        public string TableNumber { get; set; }
    }
}
