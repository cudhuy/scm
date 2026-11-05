using System.ComponentModel.DataAnnotations;

namespace Auth.API.Dtos
{
    public class VerifyEmailDto
    {
        [Required(ErrorMessage = "OTP không được để trống.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP phải gồm 6 chữ số.")]
        public string Otp { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email length cannot exceed 100 characters.")]
        public string Email { get; set; }
    }
}
