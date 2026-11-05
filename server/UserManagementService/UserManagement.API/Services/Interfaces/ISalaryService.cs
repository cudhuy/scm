using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Services.Interfaces
{
    public interface ISalaryService
    {
        Task <bool> ConfirmMonthlySalary(List<CreateMonthlySalaryDto> request);
        Task<List<MonthlySalaryResponse>> GetSalaryList(GetSalaryDto salaryDto);
        Task<bool> SendSalarySlip(SendSalarySlipDto request);
    }

}