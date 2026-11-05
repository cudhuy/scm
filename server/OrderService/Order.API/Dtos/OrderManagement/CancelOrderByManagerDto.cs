using System.ComponentModel.DataAnnotations;

namespace Order.API.Dtos.OrderManagement
{
    public class CancelOrderByManagerDto
    {
        [MaxLength(255)]
        public string? Reason { get; set; }

        public Guid? CancelReasonId { get; set; }
    }
}
