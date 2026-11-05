using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace UserManagement.API.Dtos
{
    public class CreateNewVoucherDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        public int NoOfVoucher { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public float DiscountValue { get; set; }

        [Required]
        public VoucherEnum.EDiscountType DiscountType { get; set; }

        [Required]
        public float MinOrder { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        [Required]
        public List<Guid> MembershipRankIds { get; set; }
    }
}
