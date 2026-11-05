using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class EmployeeAttendanceRepository : IEmployeeAttendanceRepository
    {
        private readonly ILogger<EmployeeAttendanceRepository> _logger;
        private readonly SCMDbContext _context;

        public EmployeeAttendanceRepository(ILogger<EmployeeAttendanceRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Branch> GetBranchByEmployeeShiftId(Guid id)
        {
            _logger.LogInformation("Entered method GetBranchByEmployeeId EmployeeAttendanceRepository");
            var result = await _context.EmployeeShifts
                .AsNoTracking()
                .Where(item => item.Id == id)
                .Select(item => item.Branch)
                .FirstOrDefaultAsync();
            _logger.LogInformation("Leaving method GetBranchByEmployeeId EmployeeAttendanceRepository");
            return result;
        }

        public async Task<EmployeeShift> GetEmployeeShiftById(Guid employeeShiftId)
        {
            _logger.LogInformation("Entered method GetEmployeeShiftById EmployeeAttendanceRepository");
            var result = await _context.EmployeeShifts.Include(item => item.WorkShift).FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == employeeShiftId);
            _logger.LogInformation("Leaving method GetEmployeeShiftById EmployeeAttendanceRepository");
            return result;
        }

        public async Task<List<EmployeeShift>> GetIndividualAttendance(Guid employeeId)
        {
            _logger.LogInformation("Entered method GetIndividualAttendance EmployeeAttendanceRepository");
            var fromDate = DateTime.Now.AddMonths(-1);
            var result = await _context.EmployeeShifts.AsNoTracking()
                .Where(item => item.EmployeeId == employeeId && item.WorkDate >= fromDate && !item.IsDeleted)
                .Include(item => item.Employee).ToListAsync();
            _logger.LogInformation("Leaving method GetIndividualAttendance EmployeeAttendanceRepository");
            return result;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
