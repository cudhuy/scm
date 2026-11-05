using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class EmployeeShift : BaseEntity
    {
        [Required]
        public Guid WorkShiftId { get; set; }
        public virtual WorkShift WorkShift { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }
        public virtual User Employee { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        public string Status { get; set; } // Not Started, Completed, Started, Late, Absent

        public decimal StartAmount { get; set; }
        public decimal EndAmount { get; set; }

        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        [Required]
        public Guid BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

    }
}
