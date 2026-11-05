using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Domain.Context;
using Domain.Entities;
using UserManagement.API.Dtos;
using UserManagement.API.Repositories.Behaviours;

namespace UserManagementTest.UnitTests.Repositories
{
    public class UserRolesRepositoryTests
    {
        private DbContextOptions<SCMDbContext> _dbContextOptions;

        public UserRolesRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private UserRolesRepository GetRepository(SCMDbContext context)
        {
            var logger = new Mock<ILogger<UserRolesRepository>>();
            return new UserRolesRepository(logger.Object, context);
        }

        [Fact]
        public async Task CreateUserRole_ShouldReturnTrue_WhenValid()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            var repo = GetRepository(context);

            var role = new UserRole { Id = Guid.NewGuid(), RoleName = "Admin" };

            var result = await repo.CreateUserRole(role);

            Assert.True(result);
            Assert.Single(await context.UserRoles.ToListAsync());
        }

        [Fact]
        public async Task DeleteUserRole_ShouldReturnFalse_WhenRoleNotFound()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            var repo = GetRepository(context);

            var result = await repo.DeleteUserRole(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserRole_ShouldReturnTrue_WhenRoleExists()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            var role = new UserRole { Id = Guid.NewGuid(), RoleName = "User" };
            context.UserRoles.Add(role);
            await context.SaveChangesAsync();

            var repo = GetRepository(context);

            var result = await repo.DeleteUserRole(role.Id);

            Assert.True(result);
            Assert.True((await context.UserRoles.FindAsync(role.Id)).IsDeleted);
        }

        [Fact]
        public async Task UpdateUserRole_ShouldReturnFalse_WhenRoleNotFound()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            var repo = GetRepository(context);

            var result = await repo.UpdateUserRole(Guid.NewGuid(), new UserRole());

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserRole_ShouldReturnTrue_WhenValid()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            var role = new UserRole { Id = Guid.NewGuid(), RoleName = "OldRole" };
            context.UserRoles.Add(role);
            await context.SaveChangesAsync();

            var updated = new UserRole { RoleName = "NewRole", Description = "Updated" };
            var repo = GetRepository(context);

            var result = await repo.UpdateUserRole(role.Id, updated);

            Assert.True(result);
            var updatedRole = await context.UserRoles.FindAsync(role.Id);
            Assert.Equal("NewRole", updatedRole.RoleName);
        }

        [Fact]
        public async Task GetAllUserRoles_ShouldReturnOnlyNotDeleted()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            context.UserRoles.AddRange(
                new UserRole { Id = Guid.NewGuid(), RoleName = "A", IsDeleted = false },
                new UserRole { Id = Guid.NewGuid(), RoleName = "B", IsDeleted = true }
            );
            await context.SaveChangesAsync();

            var repo = GetRepository(context);

            var result = await repo.GetAllUserRoles();

            Assert.Single(result);
            Assert.Equal("A", result[0].RoleName);
        }

        [Fact]
        public async Task GetRoleById_ShouldReturnCorrectRole()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            var role = new UserRole { Id = Guid.NewGuid(), RoleName = "Tester" };
            context.UserRoles.Add(role);
            await context.SaveChangesAsync();

            var repo = GetRepository(context);

            var result = await repo.GetRoleById(role.Id);

            Assert.NotNull(result);
            Assert.Equal("Tester", result.RoleName);
        }

        [Fact]
        public async Task GetAllRolesWithUsers_ShouldReturnRoles()
        {
            using var context = new SCMDbContext(_dbContextOptions);
            context.UserRoles.Add(new UserRole
            {
                Id = Guid.NewGuid(),
                RoleName = "WithUsers",
                Users = new List<User>()
            });
            await context.SaveChangesAsync();

            var repo = GetRepository(context);
            var result = await repo.GetAllRolesWithUsers();

            Assert.Single(result);
        }
    }
}
