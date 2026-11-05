using AutoMapper;
using Domain.Entities;
using UserManagement.API.Dtos.Customer;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ILogger<CustomerService> logger, ICustomerRepository customerRepository, IMapper mapper)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<GetCustomersResponseDto> GetAllCustomers(int page, int? pageSize, string? search, Guid? rankId)
        {
            try
            {
                var customerList = await _customerRepository.GetListCustomers();

                if (customerList == null || !customerList.Any())
                {
                    _logger.LogInformation("No staff found.");
                    return new GetCustomersResponseDto
                    {
                        Items = Enumerable.Empty<CustomerDto>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize ?? 10,
                        TotalPages = 0
                    };
                }
                if (!string.IsNullOrWhiteSpace(search)) customerList = customerList.Where(st => st.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (rankId.HasValue) customerList = customerList.Where(st => st.RankId == rankId.Value).ToList();

                var totalItems = customerList.Count();
                var size = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling(totalItems / (double)size);

                var paginatedCustomers = customerList
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();

                var customerDto = _mapper.Map<List<CustomerDto>>(paginatedCustomers)
                    .Select((c, index) =>
                    {
                        c.CreatedAt = paginatedCustomers[index].CreatedAt.ToString("dd-MM-yyyy");
                        c.Dob = paginatedCustomers[index].Dob?.ToString("dd-MM-yyyy");
                        return c;
                    })
                    .ToList();

                var pagedCustomers = customerDto;

                _logger.LogInformation("Successfully retrieved {TotalItems} customers.", totalItems);

                return new GetCustomersResponseDto
                {
                    Items = customerDto,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = size,
                    TotalPages = totalPages
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllCustomers)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<User> GetCustomerById(Guid id)
        {
            return await _customerRepository.GetCustomerById(id);
        }

        public async Task<bool> UpdateCustomer(CustomerUpdateDto customer)
        {
            return await _customerRepository.UpdateCustomer(customer);
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
        {
            var user = await _customerRepository.GetCustomerById(userId);
            if (user == null)
            {
                _logger.LogInformation("User not found.");
                return null;
            }
            var totalOrders = await _customerRepository.CountTotalOrders(userId);
            var pendingOrders = await _customerRepository.CountPendingOrders(userId);
            var completedOrders = await _customerRepository.CountCompletedOrders(userId);
            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Dob = user.Dob,
                Address = user.Address,
                IsVerified = user.IsVerified,
              //  RoleName = user.Role?.Name,
                RankName = user.Ranks?.Name,
                Avatar = user.Avatar,
                Status = user.Status,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders
            };
        }
    }
}
