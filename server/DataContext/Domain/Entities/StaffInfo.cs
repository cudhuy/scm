using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class StaffInfo : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Required]
        public string Tittle { get; set; }

        [Required]
        public string Type { get; set; } // Hourly or Fixed Salary

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalaryBase { get; set; }

        [Required]
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
