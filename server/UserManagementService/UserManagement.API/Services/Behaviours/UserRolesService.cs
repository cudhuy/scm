using AutoMapper;
using UserManagement.API.Dtos;
using Domain.Entities;
using UserManagement.API.Repositories.Behaviours;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class UserRolesService : IUserRolesService
    {
        private readonly ILogger<UserRolesService> _logger;
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly IMapper _mapper;


        public UserRolesService(ILogger<UserRolesService> logger, IUserRolesRepository userRolesRepository, IMapper mapper)
        {
            _logger = logger;
            _userRolesRepository = userRolesRepository;
            _mapper = mapper;

        }

        public Task<List<UserRoleDto>> GetAllUserRoles()
        {
            _logger.LogInformation("Entered method GetAllUserRoles");
            var result = _userRolesRepository.GetAllUserRoles();
            _logger.LogInformation("Leaving method GetAllUserRoles");
            return result;
        }
        public async Task<bool> CreateUserRole(CreateRoleDto createUserRoleDto)
        {
            _logger.LogInformation("Creating a new user role. 'UserRolesService'");

            var userRole = _mapper.Map<UserRole>(createUserRoleDto);
            var result = await _userRolesRepository.CreateUserRole(userRole);

            if (!result)
            {
                _logger.LogWarning("Failed to create user role.'UserRolesService'");
                return false;
            }

            _logger.LogInformation("User role created successfully.'UserRolesService'");
            return true;
        }


        public async Task<bool> DeleteUserRole(Guid id)
        {
            _logger.LogInformation("Entered method DeleteUserRole in service for role ID {Id} 'UserRolesService'", id);

            try
            {
                var result = await _userRolesRepository.DeleteUserRole(id);
                if (!result)
                {
                    _logger.LogWarning("Staff with ID {Id} not found in service.'UserRolesService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully marked staff with ID {Id} as deleted in service.'UserRolesService'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(DeleteUserRole)}: {ex.Message}, StackTrace: {ex.StackTrace}'UserRolesService'");
                throw;
            }
        }

        public async Task<bool> UpdateUserRole(Guid id, UpdateRoleDto updateRoleDto)
        {
            _logger.LogInformation("Entered UpdateUserRole  in service for role with id {id}.'UserRolesService'", id);

            try
            {
                var existingRole = await _userRolesRepository.GetRoleById(id);
                if (existingRole == null)
                {
                    _logger.LogWarning("role with id {id} not found.'UserRolesService'", id);
                    return false;
                }


                var updatedRole = _mapper.Map<UserRole>(updateRoleDto);
                updatedRole.Id = existingRole.Id;

                var resultRoleUpdate = await _userRolesRepository.UpdateUserRole(id, updatedRole);

                if (resultRoleUpdate == null)
                {
                    _logger.LogWarning("Failed to update role with id {Id}.'UserRolesService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully updated role with id {Id}.'UserRolesService'", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(UpdateUserRole)}: {ex.Message}, StackTrace: {ex.StackTrace}.'UserRolesService'");
                throw;
            }
        }
        public async Task<List<RoleCountDto>> GetRoleUserCount()
        {
            try
            {
                _logger.LogInformation("Entered GetRoleUserCount  in service for role with id {id}.'UserRolesService'");

                var roles = await _userRolesRepository.GetAllRolesWithUsers();
                var roleCounts = roles.Select(role => new RoleCountDto
                {
                    RoleName = role.RoleName,
                    UserCount = role.Users.Count()  
                }).ToList();

                return roleCounts;
                _logger.LogInformation("Exit GetRoleUserCount  in service for role with id {id}.'UserRolesService'");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while counting users by role.'UserRolesService'");
                throw new Exception("Failed to get role user counts'UserRolesService'", ex);
            }
        }


    }
}
