using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Voucher : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        public int NoOfVoucher { get; set; }

        [Required]
        public int noOfVoucherUsed { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public float DiscountValue { get; set; }

        [Required]
        public int DiscountType { get; set; } // 0: PERCENTAGE, 1: FIXED_AMOUNT

        [Required]
        public float MinOrder { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        public virtual ICollection<VoucherUsage> VoucherUsages { get; set; }
        public virtual ICollection<MembershipRanksVouchers> MembershipRanksVouchers { get; set; }

    }
}
