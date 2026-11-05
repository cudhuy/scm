using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class OrderCancellation : BaseEntity
    {
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey("CancelReason")]
        public Guid? CancelReasonId { get; set; }
        public CancelReason? CancelReason { get; set; }

        [MaxLength(255)]
        public string CustomReason { get; set; }

        public Guid CanceledBy { get; set; }
    }
}
