using System.ComponentModel.DataAnnotations;

namespace Auth.API.Dtos
{
    public class GoogleLoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email length cannot exceed 100 characters.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "FullName must be between 3 and 50 characters.")]
        public string Name { get; set; }

    }
}
