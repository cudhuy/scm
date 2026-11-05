using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Salary : BaseEntity
    {
        [Required]
        public Guid EmployeeId { get; set; }
        public virtual User Employee { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public int TotalHours { get; set; }
        public decimal Bonus { get; set; }
        public decimal Penalty { get; set; }
        public decimal NetSalary { get; set; }
        public int TotalNumberOfShifts { get; set; }
        public int TotalLateShifts { get; set; }
        public int TotalAbsentShifts { get; set; }
        public int TotalNotCompleteShifts { get; set; }
        public decimal SalaryBase { get; set; }
        public bool IsConfirmed { get; set; }
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
