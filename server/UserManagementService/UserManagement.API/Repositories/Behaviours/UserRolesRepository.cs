using Domain.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using UserManagement.API.Dtos;
using UserManagement.API.Repositories.Interfaces;
using Domain.Constanst;
using StackExchange.Redis;

namespace UserManagement.API.Repositories.Behaviours
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly ILogger<UserRolesRepository> _logger;
        private readonly SCMDbContext _context;

        public UserRolesRepository(ILogger<UserRolesRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Task<List<UserRoleDto>> GetAllUserRoles()
        {
            return _context.UserRoles.Where(x => !x.IsDeleted).Select(x => new UserRoleDto
            {
                Id = x.Id,
                RoleName = x.RoleName,
                Description = x.Description
            }).ToListAsync();
        }

        public async Task<bool> CreateUserRole(UserRole userRole)
        {
            _logger.LogInformation("Entered method CreateUserRole in UserRolesRepository");

            try
            {
                userRole.CreatedAt = DateTime.Now;
                userRole.UpdatedAt = DateTime.Now;
                userRole.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                userRole.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;

                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added new user role with ID {Id}.", userRole.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the user role.");
                return false;
            }
        }

        public async Task<bool> DeleteUserRole(Guid id)
        {
            _logger.LogInformation("Entered method DeleteUserRole in 'UserRolesRepository'");

            try
            {
                var role = await _context.UserRoles.FirstOrDefaultAsync(v => v.Id == id);
                if (role == null)
                {
                    _logger.LogWarning("Role with ID {Id} not found.'UserRolesRepository'", id);
                    return false;
                }

                role.IsDeleted = true;
                _context.UserRoles.Update(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully marked role with ID {Id} as deleted. 'UserRolesRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the voucher as deleted. 'UserRolesRepository'");
                throw;
            }
        }

        public async Task<bool> UpdateUserRole(Guid id, UserRole updatedUserRole)
        {
            _logger.LogInformation("Entered method UpdateUserRole in 'UserRolesRepository'");

            try
            {
                var role = await _context.UserRoles.FirstOrDefaultAsync(v => v.Id == id);
                if (role == null)
                {
                    _logger.LogWarning("role with ID {Id} not found to update. 'UserRolesRepository'", id);
                    return false;
                }

                role.RoleName = updatedUserRole.RoleName;
                role.Description = updatedUserRole.Description;
                role.UpdatedAt = DateTime.Now;

                _context.UserRoles.Update(role);
                await _context.SaveChangesAsync();
              await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated role with ID {Id}.", id);
                return true; // Successfully updated
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the role.");
                return false; // Failed to update
            }
        }

        public async Task<UserRole> GetRoleById(Guid id)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<UserRole>> GetAllRolesWithUsers()
        {
            return await _context.UserRoles
                                 .Include(r => r.Users) 
                                 .ToListAsync();
        }

    }
}
