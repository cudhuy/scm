using Domain.Entities;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Dtos.Staff;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface IStaffInformationRepository
    {
        Task<List<StaffInfo>> GetListStaffInfor();
        Task<bool> DeleteStaff(Guid id);
        Task<bool> UpdateStaff(UpdateStaffDto updatedStaffInfo);
        Task<StaffInfo> GetStaffById(Guid id);
        Task<bool> CreateStaff(CreateStaffDto createStaffDto);
        Task<bool> Save();
        Task<StaffInfo> GetByUserId(Guid userId);
        Task<List<StaffInfo>> GetByUserIds(List<Guid> employeeIds);
        Task<User?> GetUserByIdAsync(Guid id);
        //staff profile 
        Task<bool> UpdateProfile(UpdateProfileDto updateProfileDto, string? imageUrl);
        Task<User?> GetUserById(Guid id);
        Task<User> GetUserStaffById(Guid id); 
    }
}
