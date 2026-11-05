using Domain.Constanst;
using Domain.Context;
using Microsoft.EntityFrameworkCore;
using Product.API.Repositories.Interfaces;

namespace Product.API.Repositories.Behaviours
{
    public class AlertRepository : IAlertRepository
    {

        private readonly ILogger<CategoryRepository> _logger;
        private readonly SCMDbContext _context;

        public AlertRepository(ILogger<CategoryRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<string?> GetBranchManagerEmail(string inventoryName)
        {
            var result = await _context.Branches
                 .Where(b => b.Name == inventoryName)
                 .Include(b => b.Staffs)
                     .ThenInclude(s => s.User)
                .Select(b => b.Staffs
                        .FirstOrDefault(s => s.User.RoleId == AppConstants.MANAGER_ROLE_GUID)
                        .User.Email)
                .FirstOrDefaultAsync();


            return result;
        }
    }
}
