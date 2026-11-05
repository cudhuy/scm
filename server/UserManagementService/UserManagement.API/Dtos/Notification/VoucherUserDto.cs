namespace UserManagement.API.Dtos.Notification
{
    public class VoucherUserDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public float DiscountValue { get; set; }
        public DateTime ExpireDate { get; set; }
        public float MinOrder { get; set; }

        public int DiscountType { get; set; }

    }
}
