using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Dtos.Customer;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;
using Xunit;

namespace UserManagementTest.UnitTests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly Mock<ICustomerRepository> _repoMock;
        private readonly IMapper _mapper;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _repoMock = new Mock<ICustomerRepository>();

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, CustomerDto>();
            });
            _mapper = config.CreateMapper();

            _service = new CustomerService(_loggerMock.Object, _repoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllCustomers_ReturnsPagedResult_WhenDataExists()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FullName = "Alice", CreatedAt = DateTime.UtcNow, Dob = DateTime.UtcNow.AddYears(-20) },
                new User { Id = Guid.NewGuid(), FullName = "Bob", CreatedAt = DateTime.UtcNow, Dob = DateTime.UtcNow.AddYears(-25) }
            };

            _repoMock.Setup(r => r.GetListCustomers()).ReturnsAsync(users);

            // Act
            var result = await _service.GetAllCustomers(1, 10, null, null);

            // Assert
            Assert.Equal(2, result.TotalItems);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal("Alice", result.Items.First().FullName);
        }

        [Fact]
        public async Task GetAllCustomers_ReturnsEmpty_WhenNoData()
        {
            _repoMock.Setup(r => r.GetListCustomers()).ReturnsAsync(new List<User>());

            var result = await _service.GetAllCustomers(1, 10, null, null);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsUser_WhenFound()
        {
            var userId = Guid.NewGuid();
            var expectedUser = new User { Id = userId, FullName = "Test User" };

            _repoMock.Setup(r => r.GetCustomerById(userId)).ReturnsAsync(expectedUser);

            var result = await _service.GetCustomerById(userId);

            Assert.Equal(expectedUser.FullName, result.FullName);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsTrue_WhenSuccess()
        {
            var updateDto = new CustomerUpdateDto();
            _repoMock.Setup(r => r.UpdateCustomer(updateDto)).ReturnsAsync(true);

            var result = await _service.UpdateCustomer(updateDto);

            Assert.True(result);
        }

        [Fact]
        public async Task GetUserProfileAsync_ReturnsProfile_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FullName = "Test",
                Email = "test@email.com",
                Dob = DateTime.UtcNow,
                Ranks = new MembershipRanks { Name = "Silver" },
                Status = "Active",
                Phone = "123456",
                Address = "123 Street",
                IsVerified = true,
                Avatar = "img.png"
            };

            _repoMock.Setup(r => r.GetCustomerById(userId)).ReturnsAsync(user);
            _repoMock.Setup(r => r.CountTotalOrders(userId)).ReturnsAsync(5);
            _repoMock.Setup(r => r.CountPendingOrders(userId)).ReturnsAsync(2);
            _repoMock.Setup(r => r.CountCompletedOrders(userId)).ReturnsAsync(3);

            var result = await _service.GetUserProfileAsync(userId);

            Assert.NotNull(result);
            Assert.Equal("Silver", result.RankName);
            Assert.Equal(5, result.TotalOrders);
        }

        [Fact]
        public async Task GetUserProfileAsync_ReturnsNull_WhenUserNotFound()
        {
            _repoMock.Setup(r => r.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var result = await _service.GetUserProfileAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}
