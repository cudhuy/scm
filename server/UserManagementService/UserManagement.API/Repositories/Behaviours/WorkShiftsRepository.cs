using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class WorkShiftsRepository : IWorkShiftsRepository
    {
        private readonly ILogger<WorkShiftsRepository> _logger;
        private readonly SCMDbContext _context;

        public WorkShiftsRepository(ILogger<WorkShiftsRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<List<WorkShift>> GetShifts()
        {
            _logger.LogInformation("Entered method GetShifts WorkShiftsRepository");
            var result = await _context.WorkShifts.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.StartTime).ToListAsync();
            _logger.LogInformation("Leaving method GetShifts WorkShiftsRepository");
            return result;
        }

        public async Task<WorkShift> GetShiftById(Guid id)
        {
            _logger.LogInformation("Entered method GetShiftById WorkShiftsRepository");
            var result = await _context.WorkShifts.FirstOrDefaultAsync(x => x.Id == id);
            _logger.LogInformation("Leaving method GetShiftById WorkShiftsRepository");
            return result;
        }
    }
}
