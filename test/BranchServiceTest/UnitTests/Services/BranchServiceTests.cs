using AutoMapper;
using Branch.API.Dtos.Branch;
using Branch.API.Services.Behaviours;
using Branch.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Services
{
    public class BranchServiceTests
    {
        private readonly Mock<IBranchRepository> _branchRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BranchService>> _loggerMock;
        private readonly BranchService _branchService;

        public BranchServiceTests()
        {
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BranchService>>();
            _branchService = new BranchService(_branchRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllBranch_ShouldReturnListOfBranchDto_WhenBranchesExist()
        {
            // Arrange
            var branches = new List<Domain.Entities.Branch>
        {
            new Domain.Entities.Branch { Id = Guid.NewGuid(), Name = "Branch 1", Address = "Address 1", Email = "a@a.com", Phone = "123", Type = "Main" },
        };

            _branchRepositoryMock.Setup(r => r.GetListBranch()).ReturnsAsync(branches);

            // Act
            var result = await _branchService.GetAllBranch();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateBranch_ShouldReturnTrue_WhenCreateSuccess()
        {
            // Arrange
            var dto = new CreateBranchDto { Name = "New Branch", Address = "Address", Email = "b@b.com", Phone = "456" };
            var branch = new Domain.Entities.Branch();

            _mapperMock.Setup(m => m.Map<Domain.Entities.Branch>(dto)).Returns(branch);
            _branchRepositoryMock.Setup(r => r.CreateBranch(branch)).ReturnsAsync(true);

            // Act
            var result = await _branchService.CreateBranch(dto);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateBranch_ShouldReturnTrue_WhenUpdateSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateBranchDto { Name = "Updated", Address = "Addr", Email = "u@u.com", Phone = "999" };
            var existing = new Domain.Entities.Branch { Id = id, Name = "Old" };
            var updated = new Domain.Entities.Branch();

            _branchRepositoryMock.Setup(r => r.GetBranchById(id)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map<Domain.Entities.Branch>(dto)).Returns(updated);
            _branchRepositoryMock.Setup(r => r.UpdateBranch(id, updated)).ReturnsAsync(true);

            // Act
            var result = await _branchService.UpdateBranch(id, dto);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteBranch_ShouldReturnFalse_WhenBranchNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _branchRepositoryMock.Setup(r => r.GetBranchById(id)).ReturnsAsync((Domain.Entities.Branch)null);

            // Act
            var result = await _branchService.DeleteBranch(id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetBranchById_ShouldReturnDto_WhenFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var branch = new Domain.Entities.Branch { Id = id, Name = "Branch" };
            var dto = new BranchDetailDto { Name = "Branch" };

            _branchRepositoryMock.Setup(r => r.GetBranchById(id)).ReturnsAsync(branch);
            _mapperMock.Setup(m => m.Map<BranchDetailDto>(branch)).Returns(dto);

            // Act
            var result = await _branchService.GetBranchById(id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Branch");
        }
    }
}
