using Domain.Constanst;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.API.Dtos.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Dob { get; set; }
        public bool IsVerified { get; set; }
        public Guid RankId { get; set; }
        public string RankName => AppConstants.GetMembershipName(RankId);
        public string Avatar { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string CreatedAt { get; set; }
    }

    public class CustomerUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        public bool IsVerified { get; set; }
        [Required]
        public Guid RankId { get; set; }
        [Required]
        [RegularExpression("Active|Inactive", ErrorMessage = "Status chỉ có thể là 'Active' hoặc 'Inactive'.")]
        [Column(TypeName = "enum('Active','Inactive')")]
        public string Status { get; set; }
    }
}
