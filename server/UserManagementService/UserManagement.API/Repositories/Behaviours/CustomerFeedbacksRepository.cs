using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Repositories.Interfaces;
namespace UserManagement.API.Repositories.Behaviours
{
    public class CustomerFeedbacksRepository : ICustomerFeedbacksRepository
    {
        private readonly SCMDbContext _context;
        private readonly ILogger<CustomerFeedbacksRepository> _logger;

        public CustomerFeedbacksRepository(SCMDbContext context, ILogger<CustomerFeedbacksRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<Feedback>> GetAllFeedbackCustomer()
        {
            _logger.LogInformation("Entered method GetAllFeedbackCustomer in CustomerFeedbacksRepository");
            try
            {
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.User) 
                    .ThenInclude(u => u.Ranks) 
                    .Where(f => !f.IsDeleted && f.User.StaffInfo == null) 
                    .ToListAsync();

                _logger.LogInformation($"Successfully retrieved {feedbacks.Count} feedbacks.");

                return feedbacks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving feedbacks.");
                throw;
            }
        }


    }
}
