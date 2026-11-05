using Domain.Entities;
using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface IEmployeeShiftsRepository
    {
        Task<bool> AddEmployeeShift(EmployeeShift entity);
        Task<bool> DeleteEmployeeShift(EmployeeShift entity);
        Task<List<EmployeeShift>> GetEmployeeShiftByDateAndBranch(GetSalaryDto salaryDto);
        Task<EmployeeShift?> GetEmployeeShiftById(Guid id);
        Task<List<User>> GetStaffs(Guid branchId);
        Task<List<EmployeeShift>> GetStaffScheduleByDate(StaffScheduleParameter input);
        Task<List<EmployeeShift>> GetStaffSchedulesLastMonth(Guid branchId);
    }
}
