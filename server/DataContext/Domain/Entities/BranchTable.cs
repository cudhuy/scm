using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class BranchTable : BaseEntity
    {
        [Required, StringLength(50)]
        public string TableNumber { get; set; } // Số bàn

        [Required]
        public bool IsOccupied { get; set; } = false; // Bàn đang có khách hay không

        [Required]
        [ForeignKey("Branch")]
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
