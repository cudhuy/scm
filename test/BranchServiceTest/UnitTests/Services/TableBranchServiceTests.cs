using Branch.API.Dtos.TableBranch;
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
    public class TableBranchServiceTests
    {
        private readonly Mock<ITableBranchRepository> _mockRepo;
        private readonly Mock<ILogger<TableBranchRepository>> _mockLogger;
        private readonly TableBranchService _service;

        public TableBranchServiceTests()
        {
            _mockRepo = new Mock<ITableBranchRepository>();
            _mockLogger = new Mock<ILogger<TableBranchRepository>>();
            _service = new TableBranchService(_mockRepo.Object, null, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTable_ShouldReturnFalse_WhenTableAlreadyExists()
        {
            // Arrange
            var dto = new CreateTableDto { BranchId = Guid.NewGuid(), TableNumber = "10" };
            _mockRepo.Setup(r => r.GetBranchById(dto.BranchId)).ReturnsAsync(new Domain.Entities.Branch());
            _mockRepo.Setup(r => r.GetByBranchAndTableNumber(dto.BranchId, dto.TableNumber))
                     .ReturnsAsync(new BranchTable());

            // Act
            var result = await _service.CreateTable(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ToggleTableStatus_ShouldThrow_WhenTableNotFound()
        {
            // Arrange
            var tableId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetTableByIdAsync(tableId)).ReturnsAsync((BranchTable)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ToggleTableStatus(tableId));
        }

        [Fact]
        public async Task DeleteBranch_ShouldReturnFalse_WhenTableNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetTableByIdAsync(id)).ReturnsAsync((BranchTable)null);

            // Act
            var result = await _service.DeleteBranch(id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateTable_ShouldReturnFalse_WhenTableNumberAlreadyExists()
        {
            // Arrange
            var tableId = Guid.NewGuid();
            var existingTable = new BranchTable { Id = tableId, BranchId = Guid.NewGuid() };
            _mockRepo.Setup(r => r.GetTableByIdAsync(tableId)).ReturnsAsync(existingTable);
            _mockRepo.Setup(r => r.GetByBranchAndTableNumberToUpdate(existingTable.BranchId, "10", tableId))
                     .ReturnsAsync(new List<BranchTable> { new BranchTable() });

            // Act
            var result = await _service.UpdateTable(tableId, new UpdateTableDto { TableNumber = "10" });

            // Assert
            Assert.False(result);
        }
    }
}
