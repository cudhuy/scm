namespace UserManagement.API.Dtos
{
    public class VoucherUsagesDto
    {
       // public Guid VoucherUsageId { get; set; }
        public Guid VoucherId { get; set; }
        public string VoucherDescription { get; set; }
        public float DiscountValue { get; set; }
        public int DiscountType { get; set; } // 0: PERCENTAGE, 1: FIXED_AMOUNT
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime UsedAt { get; set; }

        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
    }
}
