using System.ComponentModel.DataAnnotations;

namespace Order.API.Dtos.CancelReason
{
    public class CancelOrderRequestDto
    {
        [MaxLength(255)]
        public string? Reason { get; set; } 

        public Guid? CancelReasonId { get; set; } 

    }
}
