using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CancelReason : BaseEntity
    {
        [Required, MaxLength(255)]
        public string Reason { get; set; }
    }
}
