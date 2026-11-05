using UserManagement.API.Dtos.MembershipRank;
using Domain.Enums;

namespace UserManagement.API.Dtos
{
    public class VoucherDto
    {
        public Guid VoucherId { get; set; }
        public string Code { get; set; }
        public int NoOfVoucher { get; set; }
        public string Description { get; set; }
        public float DiscountValue { get; set; }
        public VoucherEnum.EDiscountType DiscountType { get; set; }
        public float MinOrder { get; set; }
        public int UsageCount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        //  public List<Guid> MembershipRankIds { get; set; }
        public List<MembershipRankOfVoucherDto> MembershipRanks { get; set; }
    }
}
