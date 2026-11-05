using UserManagement.API.Dtos;
using Domain.Entities;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface IUserRolesRepository
    {
        Task<List<UserRoleDto>> GetAllUserRoles();
        Task<bool> CreateUserRole(UserRole userRole);
        Task<bool> DeleteUserRole(Guid id);
        Task<bool> UpdateUserRole(Guid id, UserRole updatedUserRole);
        Task<UserRole> GetRoleById(Guid id);
        Task<List<UserRole>> GetAllRolesWithUsers();
    }
}
