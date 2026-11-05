using Domain.Entities;
using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Services.Interfaces
{
    public interface IWorkShiftsService
    {
        public Task<bool> UpdateShift(UpdateWorkShiftDto workShift);
        public Task<List<WorkShiftResponseDto>> GetShifts();
    }
}
