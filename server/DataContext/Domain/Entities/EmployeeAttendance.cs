using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class EmployeeAttendance : BaseEntity
    {
        [Required]
        public Guid EmployeeShiftId { get; set; }
        public virtual EmployeeShift EmployeeShift { get; set; }

        [Required]
        public Guid WorkShiftId { get; set; }
        public virtual WorkShift WorkShift { get; set; }

    }
}
