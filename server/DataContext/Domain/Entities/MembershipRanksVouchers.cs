using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MembershipRanksVouchers : BaseEntity
    {
        [Required]
        public Guid MembershipRankId { get; set; }
        public virtual MembershipRanks MembershipRank { get; set; }

        [Required]
        public Guid VoucherId { get; set; }
        public virtual Voucher Voucher { get; set; }

    }
}
