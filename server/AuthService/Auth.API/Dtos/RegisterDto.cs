using System.ComponentModel.DataAnnotations;

namespace Auth.API.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "FullName must be between 3 and 50 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "New password must contain at least one uppercase letter, one number, and one special character.")]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email length cannot exceed 100 characters.")]
        public string Email { get; set; }

        public bool IsLoginWithGoogle { get; set; } = false;

    }
}