using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Services.Interfaces
{
    public interface IEmployeeShiftsService
    {
        public Task<bool> AddEmployeeShift(CreateEmployeeShiftDto input);
        Task<bool> DeleteEmployeeShift(Guid id);
        Task<List<StaffInShift>> GetStaffs(Guid branchId);
        Task<StaffScheduleResponse> GetStaffScheduleByDate(StaffScheduleParameter input);
        Task<List<StaffScheduleResponse>> GetStaffSchedulesLastMonth(Guid branchId);
    }
}
