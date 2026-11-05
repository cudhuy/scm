using Domain.Entities;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface IWorkShiftsRepository
    {
        Task<bool> Save();
        public Task<List<WorkShift>> GetShifts();
        Task<WorkShift> GetShiftById(Guid id);
    }
}
