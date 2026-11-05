using Domain.Context;
using Order.API.Repositories.Interfaces;

namespace Order.API.Repositories.Behaviours
{
    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly ILogger<PaymentsRepository> _logger;
        private readonly SCMDbContext _context;

        public PaymentsRepository(ILogger<PaymentsRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }
    }
}
