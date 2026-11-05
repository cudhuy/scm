using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MembershipRanks : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        public string? Avatar { get; set; }
        public decimal RequiredAmount { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<MembershipRanksVouchers> MembershipRanksVouchers { get; set; }

    }
}
