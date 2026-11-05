using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly ILogger<SalaryRepository> _logger;
        private readonly SCMDbContext _context;

        public SalaryRepository(ILogger<SalaryRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> AddSalaryList(List<Salary> request)
        {
            _logger.LogInformation("Entered method AddSalaryList SalaryRepository");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Salaries.AddRangeAsync(request);
                int changes = await _context.SaveChangesAsync();

                if (changes == request.Count)
                {
                    await transaction.CommitAsync();
                    _logger.LogInformation("Successfully added all salaries.");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Expected to add {request.Count} salaries but only {changes} were added.");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding salaries: {ex.Message}");
                await transaction.RollbackAsync();
                return false;
            }
            finally
            {
                _logger.LogInformation("Leaving method AddSalaryList SalaryRepository");
            }
        }


        public async Task<List<Salary>> GetSalaryList(GetSalaryDto salaryDto)
        {
            _logger.LogInformation("Entered method GetSalaryList SalaryRepository");
            var result = await _context.Salaries
                .Include(item => item.Employee)
                .AsNoTracking()
                .Where(item => !item.IsDeleted
                && item.BranchId == salaryDto.BranchId
                && item.Month == salaryDto.Month
                && item.Year == salaryDto.Year).ToListAsync();
            _logger.LogInformation("Leaving method GetSalaryList SalaryRepository");
            return result;
        }

        public async Task<List<Salary>> GetSalarySlip(List<Guid> salarySlipIds)
        {
            _logger.LogInformation("Entered method GetSalarySlip SalaryRepository");
            var result = await _context.Salaries
                .AsNoTracking()
                .Where(item => item.IsConfirmed
                && !item.IsDeleted
                && salarySlipIds.Contains(item.Id))
                .Include(item => item.Employee)
                .Include(item => item.Branch)
                .ToListAsync();
            _logger.LogInformation("Leaving method GetSalarySlip SalaryRepository");
            return result;
        }
    }
}
