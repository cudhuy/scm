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
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Behaviours;

namespace UserManagementTest.UnitTests.Repositories
{
    public class SalaryRepositoryTests
    {
        private SCMDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SCMDbContext(options);
        }

        private SalaryRepository GetRepository(SCMDbContext context)
        {
            var logger = new Mock<ILogger<SalaryRepository>>().Object;
            return new SalaryRepository(logger, context);
        }

        [Fact(Skip = "pre-existing: SalaryRepository.AddSalaryList uses Database.BeginTransactionAsync; EF Core InMemory provider does not support transactions")]
        public async Task AddSalaryList_AddsAllItemsSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            var salaries = new List<Salary>
        {
            new Salary { Id = Guid.NewGuid(), BranchId = Guid.NewGuid(), Month = 4, Year = 2025 },
            new Salary { Id = Guid.NewGuid(), BranchId = Guid.NewGuid(), Month = 4, Year = 2025 }
        };

            var result = await repo.AddSalaryList(salaries);

            Assert.True(result);
            Assert.Equal(2, await context.Salaries.CountAsync());
        }

        [Fact]
        public async Task GetSalaryList_ReturnsCorrectData()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);
            var branchId = Guid.NewGuid();

            var employee = new User { Id = Guid.NewGuid(), FullName = "Test" };

            context.Users.Add(employee);
            context.Salaries.AddRange(new List<Salary>
        {
            new Salary { Id = Guid.NewGuid(), BranchId = branchId, Month = 4, Year = 2025, IsDeleted = false, Employee = employee },
            new Salary { Id = Guid.NewGuid(), BranchId = branchId, Month = 3, Year = 2025, IsDeleted = false, Employee = employee }
        });
            await context.SaveChangesAsync();

            var result = await repo.GetSalaryList(new GetSalaryDto
            {
                BranchId = branchId,
                Month = 4,
                Year = 2025
            });

            Assert.Single(result);
            Assert.Equal(branchId, result[0].BranchId);
        }

        [Fact]
        public async Task GetSalarySlip_ReturnsOnlyConfirmedSalaries()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            var emp = new User { Id = Guid.NewGuid(), FullName = "A" };
            var branch = new Branch { Id = Guid.NewGuid(), Name = "Branch A", Address = "UnitTest", Phone = "UnitTest", Type = "UnitTest" };

            var salary1 = new Salary
            {
                Id = Guid.NewGuid(),
                IsConfirmed = true,
                IsDeleted = false,
                Employee = emp,
                Branch = branch
            };

            var salary2 = new Salary
            {
                Id = Guid.NewGuid(),
                IsConfirmed = false,
                IsDeleted = false,
                Employee = emp,
                Branch = branch
            };

            context.Users.Add(emp);
            context.Branches.Add(branch);
            context.Salaries.AddRange(salary1, salary2);
            await context.SaveChangesAsync();

            var result = await repo.GetSalarySlip(new List<Guid> { salary1.Id, salary2.Id });

            Assert.Single(result);
            Assert.True(result[0].IsConfirmed);
        }
    }
}
