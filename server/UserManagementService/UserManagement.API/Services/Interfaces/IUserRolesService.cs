using UserManagement.API.Dtos;

namespace UserManagement.API.Services.Interfaces
{
    public interface IUserRolesService
    {
        Task<List<UserRoleDto>> GetAllUserRoles();
        Task<bool> CreateUserRole(CreateRoleDto userRoleDto);
        Task<bool> DeleteUserRole(Guid id);
        Task<bool> UpdateUserRole(Guid id, UpdateRoleDto updateRoleDto);
        Task<List<RoleCountDto>> GetRoleUserCount();


    }
}
