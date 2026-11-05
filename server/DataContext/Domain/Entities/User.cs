using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("user")]
    public partial class User : BaseEntity
    {
        [StringLength(255)]
        public string? FullName { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public DateTime? Dob { get; set; }
        public string? Address { get; set; }

        [Required]
        public bool IsVerified { get; set; }

        [ForeignKey("Userrole")]
        [Column("RoleID")]
        public Guid RoleId { get; set; }
        public virtual UserRole? Role { get; set; }

        public Guid? RankId { get; set; }
        public virtual MembershipRanks Ranks { get; set; }

        public string? Avatar { get; set; }

        [Column(TypeName = "enum('Active','Inactive')")]
        public string? Status { get; set; }
        public ICollection<UserNotifications> UserNotifications { get; set; }
        public virtual StaffInfo? StaffInfo { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; }

    }
}
