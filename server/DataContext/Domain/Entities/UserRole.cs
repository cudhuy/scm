using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.Base;

namespace Domain.Entities
{
    [Table("userrole")]
    public partial class UserRole : BaseEntity
    {
        public UserRole()
        {
            Users = new HashSet<User>();
        }

        [Required]
        [StringLength(255)]
        public string? RoleName { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
