using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Dtos
{
    public class UpdateRoleDto
    {
        [Required]
        [StringLength(255)]
        public string? RoleName { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }
    }
}
