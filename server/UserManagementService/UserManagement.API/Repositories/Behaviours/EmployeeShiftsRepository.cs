using AutoMapper;
using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class EmployeeShiftsRepository : IEmployeeShiftsRepository
    {
        private readonly ILogger<EmployeeShiftsRepository> _logger;
        private readonly SCMDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeShiftsRepository(ILogger<EmployeeShiftsRepository> logger, SCMDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
            _mapper = mapper;
        }

        public async Task<bool> AddEmployeeShift(EmployeeShift entity)
        {
            _logger.LogInformation("Entered method AddEmployeeShift EmployeeShiftsRepository");
            try
            {
                await _context.EmployeeShifts.AddAsync(entity);
                bool result = await _context.SaveChangesAsync() > 0;
                _logger.LogInformation("Leaving method AddEmployeeShift EmployeeShiftsRepository with result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddEmployeeShift EmployeeShiftsRepository");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeShift(EmployeeShift entity)
        {
            _logger.LogInformation("Entered method DeleteEmployeeShift EmployeeShiftsRepository");
            entity.IsDeleted = true;
            _logger.LogInformation("Leaving method DeleteEmployeeShift EmployeeShiftsRepository");
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<EmployeeShift>> GetEmployeeShiftByDateAndBranch(GetSalaryDto salaryDto)
        {
            _logger.LogInformation("Entered method GetEmployeeShiftByDateAndBranch EmployeeShiftsRepository");
            var result = await _context.EmployeeShifts
                .AsNoTracking()
                .Where(item => !item.IsDeleted
                && item.BranchId == salaryDto.BranchId
                && item.WorkDate.Date.Month == salaryDto.Month
                && item.WorkDate.Date.Year == salaryDto.Year)
                .Include(item => item.WorkShift)
                .ToListAsync();
            _logger.LogInformation("Leaving method GetEmployeeShiftByDateAndBranch EmployeeShiftsRepository");
            return result;
        }

        public async Task<EmployeeShift> GetEmployeeShiftById(Guid id)
            => await _context.EmployeeShifts.FirstOrDefaultAsync(item => item.Id == id);


        public async Task<List<User>> GetStaffs(Guid branchId)
        {
            _logger.LogInformation("Entered method GetStaffs EmployeeShiftsRepository");
            var result = await _context.StaffInfos.AsNoTracking()
                .Where(item => item.BranchId == branchId && !item.IsDeleted && !item.User.IsDeleted && item.User.RoleId == AppConstants.STAFF_ROLE_GUID)
                .Include(item => item.User).Select(item => item.User).ToListAsync();
            _logger.LogInformation("Leaving method GetStaffs EmployeeShiftsRepository");
            return result;
        }

        public async Task<List<EmployeeShift>> GetStaffScheduleByDate(StaffScheduleParameter input)
        {
            _logger.LogInformation("Entered method GetStaffs EmployeeShiftsRepository");
            var result = await _context.EmployeeShifts
                .AsNoTracking()
                .Where(item => item.BranchId == input.BranchId && item.WorkDate.Date == input.WorkDate.Date && !item.IsDeleted)
                .Include(item => item.Employee).ToListAsync();
            _logger.LogInformation("Entered method GetStaffs EmployeeShiftsRepository");
            return result;
        }

        public async Task<List<EmployeeShift>> GetStaffSchedulesLastMonth(Guid branchId)
        {
            _logger.LogInformation("Entered method GetStaffSchedulesLastMonth EmployeeShiftsRepository");
            var fromDate = DateTime.Now.AddMonths(-1);
            var result = await _context.EmployeeShifts.AsNoTracking().Where(item => item.BranchId == branchId && item.WorkDate >= fromDate && !item.IsDeleted)
                .Include(item => item.Employee).ToListAsync();
            _logger.LogInformation("Leaving method GetStaffSchedulesLastMonth EmployeeShiftsRepository");
            return result;
        }
    }
}
