using Domain.Entities;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface IEmployeeAttendanceRepository
    {
        Task<Branch> GetBranchByEmployeeShiftId(Guid id);
        Task<EmployeeShift> GetEmployeeShiftById(Guid employeeShiftId);
        Task <List<EmployeeShift>> GetIndividualAttendance(Guid employeeId);
        Task<bool> Save();
    }
}
