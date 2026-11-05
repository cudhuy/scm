using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Dtos.Profile
{
    public class UpdateProfileDto
    {
        public Guid UserId { get; set; }
        [StringLength(255)]
        public string? FullName { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }

        public DateTime? Dob { get; set; }
        public string? Address { get; set; }
        public IFormFile? Image { get; set; }
    }
}
