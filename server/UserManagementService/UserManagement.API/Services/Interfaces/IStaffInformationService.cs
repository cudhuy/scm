using UserManagement.API.Dtos.Profile;
using UserManagement.API.Dtos.Staff;

namespace UserManagement.API.Services.Interfaces
{
    public interface IStaffInformationService
    {
        Task<GetStaffsResponseDto> GetAllStaffs(int page, int? pageSize, string? search, Guid? branchId);
        Task<bool> DeleteStaff(Guid id);
        Task<bool> UpdateStaff(UpdateStaffDto updateStaffDto);
        Task<StaffDetailDto?> GetDetailStaffAsync(Guid userId);
        Task<bool> CreateStaff(CreateStaffDto createStaffDto);
        //profile 
        Task<UserProfileDto> UpdateProfile(UpdateProfileDto updateProfileDto);
        Task<StaffProfileDto?> GetStaffProfileAsync(Guid userId);

    }
}
