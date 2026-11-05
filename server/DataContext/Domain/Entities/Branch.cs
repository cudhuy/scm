using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Branch : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [Column(TypeName = "enum('Main','Sub')")]
        public string Type { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public string? IpAddress { get; set; }

        public ICollection<StaffInfo> Staffs { get; set; } = new List<StaffInfo>();
        public ICollection<EmployeeShift> EmployeeShifts { get; set; } = new List<EmployeeShift>();
        public Inventory Inventory { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<BranchTable> BranchTables { get; set; } = new List<BranchTable>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
