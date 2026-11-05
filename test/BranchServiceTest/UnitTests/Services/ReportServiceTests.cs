using AutoMapper;
using Branch.API.Dtos.Report;
using Branch.API.Services.Behaviours;
using Branch.API.Services.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Services
{
    public class ReportServiceTests
    {
        private readonly ReportService _reportService;
        private readonly Mock<IReportRepository> _mockReportRepo = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<ILogger<ReportService>> _mockLogger = new();

        public ReportServiceTests()
        {
            _reportService = new ReportService(_mockReportRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetTotalCustomers_ShouldReturnCount()
        {
            // Arrange
            _mockReportRepo.Setup(r => r.GetTotalCustomers()).ReturnsAsync(10);

            // Act
            var result = await _reportService.GetTotalCustomers();

            // Assert
            Assert.Equal(10, result);
        }

        [Fact]
        public async Task GetTotalAmountForStatus2Orders_ShouldReturnCorrectAmount()
        {
            // Arrange
            var expectedAmount = 500m;
            _mockReportRepo.Setup(r => r.GetTotalAmountForStatus2Orders(It.IsAny<Guid?>())).ReturnsAsync(expectedAmount);

            // Act
            var result = await _reportService.GetTotalAmountForStatus2Orders(Guid.NewGuid());

            // Assert
            Assert.Equal(expectedAmount, result);
        }

        [Fact]
        public async Task GetDetailUserAsync_WithValidId_ShouldReturnUserDetail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FullName = "Test User",
                Email = "test@example.com",
                Phone = "123456",
                Dob = DateTime.Now,
                Address = "Address",
                IsVerified = true,
                Status = "UnitTest",
                Avatar = "avatar.png",
                Ranks = new MembershipRanks { Name = "Gold" }
            };

            _mockReportRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _reportService.GetDetailUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test User", result.FullName);
            Assert.Equal("Gold", result.RankName);
        }

        [Fact]
        public async Task GetRevenueReportAsync_ShouldReturnCorrectResult()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var year = 2024;

            var data = new List<MonthlyRevenueDto>
        {
            new() { Month = "January", Revenue = 1000 },
            new() { Month = "February", Revenue = 2000 }
        };

            _mockReportRepo.Setup(r => r.GetRevenueDataAsync(categoryId, year, branchId)).ReturnsAsync(data);

            // Act
            var result = await _reportService.GetRevenueReportAsync(branchId, categoryId, year);

            // Assert
            Assert.Equal(3000, result.Total);
            Assert.Equal("February", result.LeadingMonth);
            Assert.Equal(2, result.MonthlyRevenue.Count);
        }
    }
}
