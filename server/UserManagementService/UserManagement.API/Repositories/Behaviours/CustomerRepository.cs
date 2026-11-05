using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Dtos.Customer;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ILogger<CustomerRepository> _logger;

        private readonly SCMDbContext _context;

        public CustomerRepository(ILogger<CustomerRepository> logger, SCMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<User> GetCustomerById(Guid id)
        {
            _logger.LogInformation("Entered method GetCustomerById in 'CustomerRepository'");
            var user = await _context.Users.Where(u => u.Id == id && u.RoleId == AppConstants.USER_ROLE_GUID && !u.IsDeleted).FirstOrDefaultAsync();
            _logger.LogInformation("Successfully retrieved customer with ID {Id}.'CustomerRepository'", id);
            return user;
        }

        public async Task<IEnumerable<User>> GetListCustomers()
        {
            _logger.LogInformation("Entered method GetListCustomers in 'CustomerRepository'");
            var customers = await _context.Users.Where(u => u.RoleId == AppConstants.USER_ROLE_GUID && !u.IsDeleted).ToListAsync();
            return customers;
        }

        public Task<bool> UpdateCustomer(CustomerUpdateDto customer)
        {
            _logger.LogInformation("Entered method UpdateCustomer in 'CustomerRepository'");
            User customerToUpdate = _context.Users.Where(u => u.Id == customer.Id && u.RoleId == AppConstants.USER_ROLE_GUID && !u.IsDeleted).FirstOrDefault();
            if (customerToUpdate == null)
            {
                _logger.LogInformation("Customer with ID {Id} not found.'CustomerRepository'", customer.Id);
                return Task.FromResult(false);
            }
            customerToUpdate.IsVerified = customer.IsVerified;
            customerToUpdate.RankId = customer.RankId;
            customerToUpdate.Status = customer.Status;

            _context.Users.Update(customerToUpdate);
            return Save();
        }

        public async Task<bool> Save()
        {
            try
            {
                _logger.LogInformation("Entered StaffInformationRepository");
                var saved = await _context.SaveChangesAsync();
                _logger.LogInformation("Leaving StaffInformationRepository");
                return saved > 0;
            }
            catch (DbUpdateException ex)
            {
                 _logger.LogError($"Lỗi khi lưu vào database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Chi tiết lỗi: {ex.InnerException.Message}");
                }
                return false;
            }
        }
        public async Task<int> CountTotalOrders(Guid userId)
        {
            return await _context.Orders.CountAsync(o => o.User.Id == userId && !o.IsDeleted && !o.User.IsDeleted);
        }

        public async Task<int> CountPendingOrders(Guid userId)
        {
            return await _context.Orders.CountAsync(o => o.User.Id == userId && o.Status == 0 && !o.IsDeleted && !o.User.IsDeleted);
        }

        public async Task<int> CountCompletedOrders(Guid userId)
        {
            return await _context.Orders.CountAsync(o => o.User.Id == userId && o.Status == 2 && !o.IsDeleted && !o.User.IsDeleted);
        }
    }
}
