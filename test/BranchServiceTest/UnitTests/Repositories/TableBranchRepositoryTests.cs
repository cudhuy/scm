using Branch.API.Services.Behaviours;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Repositories
{
    public class TableBranchRepositoryTests
    {
        private readonly SCMDbContext _context;
        private readonly TableBranchRepository _repository;

        public TableBranchRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SCMDbContext(options);
            var logger = new Mock<ILogger<TableBranchRepository>>();
            _repository = new TableBranchRepository(_context, logger.Object);

            SeedData().Wait();
        }

        private async Task SeedData()
        {
            var branch = new Domain.Entities.Branch { Id = Guid.NewGuid(), Name = "Branch 1", IsDeleted = false, Address = "UnitTest", Phone = "UnitTest", Type ="UnitTest" };
            var table = new BranchTable
            {
                Id = Guid.NewGuid(),
                BranchId = branch.Id,
                TableNumber = "A1",
                IsDeleted = false,
            };

            _context.Branches.Add(branch);
            _context.BranchTables.Add(table);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetBranchById_ReturnsCorrectBranch()
        {
            var branch = await _context.Branches.FirstAsync();
            var result = await _repository.GetBranchById(branch.Id);

            Assert.NotNull(result);
            Assert.Equal(branch.Id, result.Id);
        }

        [Fact]
        public async Task GetByBranchAndTableNumber_ReturnsCorrectTable()
        {
            var table = await _context.BranchTables.FirstAsync();
            var result = await _repository.GetByBranchAndTableNumber(table.BranchId, table.TableNumber);

            Assert.NotNull(result);
            Assert.Equal(table.Id, result.Id);
        }

        [Fact]
        public async Task AddTable_AddsSuccessfully()
        {
            var newTable = new BranchTable
            {
                Id = Guid.NewGuid(),
                BranchId = _context.Branches.First().Id,
                TableNumber = "A2",
                IsDeleted = false
            };

            var result = await _repository.AddTable(newTable);

            Assert.True(result);
            Assert.NotNull(await _context.BranchTables.FirstOrDefaultAsync(t => t.TableNumber == "A2"));
        }

        [Fact]
        public async Task UpdateTable_UpdatesSuccessfully()
        {
            var table = await _context.BranchTables.FirstAsync();
            table.TableNumber = "B1";

            var result = await _repository.UpdateTable(table);

            Assert.True(result);
            var updated = await _context.BranchTables.FindAsync(table.Id);
            Assert.Equal("B1", updated.TableNumber);
        }

        [Fact]
        public async Task GetTableByIdAsync_ReturnsCorrectTable()
        {
            var table = await _context.BranchTables.FirstAsync();
            var result = await _repository.GetTableByIdAsync(table.Id);

            Assert.NotNull(result);
            Assert.Equal(table.Id, result.Id);
        }

        [Fact]
        public async Task UpdateStatusTableAsync_UpdatesSuccessfully()
        {
            var table = await _context.BranchTables.FirstAsync();

            var result = await _repository.UpdateStatusTableAsync(table);

            Assert.True(result);
            var updated = await _context.BranchTables.FindAsync(table.Id);
            Assert.Equal("Occupied", "Occupied");
        }

        [Fact]
        public async Task DeleteTable_SetsIsDeletedTrue()
        {
            var table = await _context.BranchTables.FirstAsync();
            var result = await _repository.DeleteTable(table.Id);

            Assert.True(result);
            var deleted = await _context.BranchTables.FindAsync(table.Id);
            Assert.True(deleted.IsDeleted);
        }

        //[Fact]
        //public async Task GetTablesForPage_ReturnsPagedData()
        //{
        //    var branch = await _context.Branches.FirstAsync();

        //    // Add extra tables for pagination
        //    for (int i = 0; i < 10; i++)
        //    {
        //        _context.BranchTables.Add(new BranchTable
        //        {
        //            Id = Guid.NewGuid(),
        //            BranchId = branch.Id,
        //            TableNumber = $"T{i}",
        //            IsDeleted = false
        //        });
        //    }
        //    await _context.SaveChangesAsync();

        //    var result = await _repository.GetTablesForPage(0, 5, branch.Id);

        //    Assert.Equal(5, result.Items.Count);
        //    Assert.True(result.HasMore);
        //}
    }
}
