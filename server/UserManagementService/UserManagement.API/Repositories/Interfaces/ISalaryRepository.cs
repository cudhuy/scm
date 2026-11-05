using Domain.Entities;
using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface ISalaryRepository
    {
        Task<bool> AddSalaryList(List<Salary> request);
        Task<List<Salary>> GetSalaryList(GetSalaryDto salaryDto);
        Task<List<Salary>> GetSalarySlip(List<Guid> salarySlipIds);
    }
}
