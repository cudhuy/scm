using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Dtos
{
    public class SaveMembershipRankDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 50 characters.")]
        public string Description { get; set; }
    }
}
