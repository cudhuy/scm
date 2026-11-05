using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class VoucherUsage : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public Guid VoucherId { get; set; }
        public virtual Voucher Voucher { get; set; }

        //có nên thêm bảng order vào UserManagement
        //public int? OrderId { get; set; }
        //public virtual Order Order { get; set; }

        [Required]
        public DateTime UsedAt { get; set; }

    }
}
