using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Feedback : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // Đánh giá từ 1 đến 5 sao

        [Required]
        public Guid BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [Required]
        public Guid OrderId { get; set; } // Mỗi đơn hàng chỉ có một phản hồi

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}
