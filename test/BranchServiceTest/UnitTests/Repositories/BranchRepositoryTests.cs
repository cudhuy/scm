using Branch.API.Services.Behaviours;
using Domain.Context;
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
    public class BranchRepositoryTests
    {
        private readonly SCMDbContext _context;
        private readonly BranchRepository _repository;

        public BranchRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SCMDbContext(options);
            var loggerMock = new Mock<ILogger<BranchRepository>>();
            _repository = new BranchRepository(_context, loggerMock.Object);
        }

        [Fact]
        public async Task GetListBranch_ReturnsBranches()
        {
            _context.Branches.Add(new Domain.Entities.Branch { Id = Guid.NewGuid(), Name = "CN1", IsDeleted = false, Address = "UnitTest", Phone = "UnitTest", Type = "UnitTest" });
            _context.Branches.Add(new Domain.Entities.Branch { Id = Guid.NewGuid(), Name = "CN2", IsDeleted = true, Address = "UnitTest", Phone = "UnitTest", Type = "UnitTest" }); // sẽ bị lọc
            await _context.SaveChangesAsync();

            var result = await _repository.GetListBranch();

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateBranch_Success()
        {
            var branch = new Domain.Entities.Branch { Id = Guid.NewGuid(), Name = "New Branch", Address = "UnitTest", Phone = "UnitTest" };

            var result = await _repository.CreateBranch(branch);

            Assert.True(result);
            var saved = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branch.Id);
            Assert.NotNull(saved);
            Assert.False(saved.IsDeleted);
            Assert.True(saved.IsActive);
            Assert.Equal("Sub", saved.Type);
        }

        [Fact]
        public async Task GetBranchById_ReturnsBranch()
        {
            var id = Guid.NewGuid();
            _context.Branches.Add(new Domain.Entities.Branch { Id = id, Name = "CN1", Address = "UnitTest", Phone = "UnitTest", Type = "UnitTest" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetBranchById(id);

            Assert.NotNull(result);
            Assert.Equal("CN1", result.Name);
        }

        [Fact]
        public async Task UpdateBranch_ExistingBranch_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            _context.Branches.Add(new Domain.Entities.Branch { Id = id, Name = "Old Name", Address = "Old Address", Phone = "UnitTest", Type = "UnitTest" });
            await _context.SaveChangesAsync();

            var updated = new Domain.Entities.Branch { Name = "New Name", Address = "New Address", Email = "email@abc.com", Phone = "123456789" };

            var result = await _repository.UpdateBranch(id, updated);

            Assert.True(result);
            var updatedBranch = await _context.Branches.FindAsync(id);
            Assert.Equal("New Name", updatedBranch.Name);
            Assert.Equal("New Address", updatedBranch.Address);
        }

        [Fact]
        public async Task UpdateBranch_NonExistingBranch_ReturnsFalse()
        {
            var result = await _repository.UpdateBranch(Guid.NewGuid(), new Domain.Entities.Branch { Name = "X" });

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteBranch_ExistingBranch_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            _context.Branches.Add(new Domain.Entities.Branch { Id = id, Name = "CN", IsDeleted = false, Address = "UnitTest", Phone = "UnitTest", Type = "UnitTest" });
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteBranch(id);

            Assert.True(result);
            var branch = await _context.Branches.FindAsync(id);
            Assert.True(branch.IsDeleted);
        }

        [Fact]
        public async Task DeleteBranch_NotFound_ReturnsFalse()
        {
            var result = await _repository.DeleteBranch(Guid.NewGuid());

            Assert.False(result);
        }
    }
}
