using System.ComponentModel.DataAnnotations;

namespace Auth.API.Dtos
{
    public class ResendOtpDto
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email length cannot exceed 100 characters.")]
        public string Email { get; set; }
    }
}
