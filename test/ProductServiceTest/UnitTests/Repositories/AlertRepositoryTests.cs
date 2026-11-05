using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product.API.Repositories.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductServiceTest.UnitTests.Repositories
{
    public class AlertRepositoryTests
    {
        private readonly DbContextOptions<SCMDbContext> _options;

        public AlertRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetBranchManagerEmail_ReturnsEmail_WhenManagerExists()
        {
            // Arrange
            var branchName = "Main Branch";
            var expectedEmail = "manager@example.com";

            using (var context = new SCMDbContext(_options))
            {
                var manager = new User
                {
                    Id = Guid.NewGuid(),
                    Email = expectedEmail,
                    RoleId = AppConstants.MANAGER_ROLE_GUID
                };

                var staff = new StaffInfo
                {
                    Id = Guid.NewGuid(),
                    User = manager,
                    Tittle = "UnitTest",
                    Type = "UnitTest"
                };

                var branch = new Branch
                {
                    Id = Guid.NewGuid(),
                    Name = branchName,
                    Staffs = new List<StaffInfo> { staff },
                    Address = "UnitTest",
                    Phone = "UnitTest",
                    Type = "UnitTest"
                };

                context.Branches.Add(branch);
                context.SaveChanges();
            }

            using (var context = new SCMDbContext(_options))
            {
                var logger = new LoggerFactory().CreateLogger<CategoryRepository>();
                var repository = new AlertRepository(logger, context);

                // Act
                var result = await repository.GetBranchManagerEmail(branchName);

                // Assert
                Assert.Equal(expectedEmail, result);
            }
        }

        [Fact]
        public async Task GetBranchManagerEmail_ReturnsNull_WhenBranchNotFound()
        {
            // Arrange
            using (var context = new SCMDbContext(_options))
            {
                context.Branches.Add(new Branch
                {
                    Id = Guid.NewGuid(),
                    Name = "Another Branch",
                    Address = "UnitTest",
                    Phone = "UnitTest",
                    Type = "UnitTest"
                });
                context.SaveChanges();
            }

            using (var context = new SCMDbContext(_options))
            {
                var logger = new LoggerFactory().CreateLogger<CategoryRepository>();
                var repository = new AlertRepository(logger, context);

                // Act
                var result = await repository.GetBranchManagerEmail("Nonexistent Branch");

                // Assert
                Assert.Null(result);
            }
        }
    }
}
