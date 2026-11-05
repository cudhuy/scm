using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Payment : BaseEntity
    {
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        [Required, MaxLength(50)]
        public string TransactionCode { get; set; }

        public int PaymentMethod { get; set; } = 0;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public int Status { get; set; } = 0;
    }
}
