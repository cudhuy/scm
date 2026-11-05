using Branch.API.Services.Interfaces;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Services.Behaviours
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly SCMDbContext _context;
        private readonly ILogger<FeedbackRepository> _logger;

        public FeedbackRepository(SCMDbContext context, ILogger<FeedbackRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Order> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId);
        }

        public async Task<bool> FeedbackExistsForOrderAsync(Guid orderId)
        {
            return await _context.Feedbacks.AnyAsync(f => f.OrderId == orderId);
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Feedback>> GetAllFeedbacksAsync(Guid? branchId)
        {
            try
            {
                var query = _context.Feedbacks
                    .Include(f => f.Order)
                    .Include(f => f.Order.User)
                    .Include(f => f.Order.Branch)
                    .Where(f => !f.IsDeleted)
                    .AsQueryable();

                if (branchId != null)
                {
                    query = query.Where(f => f.Order.BranchId == branchId);
                }

                var result = await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} feedbacks", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching feedbacks");
                return new List<Feedback>();
            }
        }
    }
}
