using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Repositories.Behaviours;

namespace UserManagementTest.UnitTests.Repositories
{
    public class EmployeeAttendanceRepositoryTests
    {
        private readonly SCMDbContext _context;
        private readonly EmployeeAttendanceRepository _repository;

        public EmployeeAttendanceRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SCMDbContext(options);
            var logger = new LoggerFactory().CreateLogger<EmployeeAttendanceRepository>();
            _repository = new EmployeeAttendanceRepository(logger, _context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                Name = "Branch A",
                Address = "UniTest",
                Phone = "1234567890",
                Type = "UnitTest"
            };

            var employee = new User
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe"
            };

            var workShift = new WorkShift
            {
                Id = Guid.NewGuid(),
                Name = "Morning Shift"
            };

            var employeeShift = new EmployeeShift
            {
                Id = Guid.NewGuid(),
                Branch = branch,
                BranchId = branch.Id,
                Employee = employee,
                EmployeeId = employee.Id,
                WorkShift = workShift,
                WorkShiftId = workShift.Id,
                WorkDate = DateTime.Now.AddDays(-10),
                IsDeleted = false,
                Status = "UnitTest"
            };

            _context.Branches.Add(branch);
            _context.Users.Add(employee);
            _context.WorkShifts.Add(workShift);
            _context.EmployeeShifts.Add(employeeShift);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetBranchByEmployeeShiftId_ReturnsCorrectBranch()
        {
            var shift = await _context.EmployeeShifts.FirstAsync();
            var branch = await _repository.GetBranchByEmployeeShiftId(shift.Id);

            Assert.NotNull(branch);
            Assert.Equal("Branch A", branch.Name);
        }

        [Fact]
        public async Task GetEmployeeShiftById_ReturnsCorrectShift()
        {
            var shift = await _context.EmployeeShifts.FirstAsync();
            var result = await _repository.GetEmployeeShiftById(shift.Id);

            Assert.NotNull(result);
            Assert.Equal(shift.Id, result.Id);
            Assert.NotNull(result.WorkShift);
        }

        [Fact]
        public async Task GetIndividualAttendance_ReturnsAttendanceList()
        {
            var employee = await _context.Users.FirstAsync();
            var result = await _repository.GetIndividualAttendance(employee.Id);

            Assert.NotEmpty(result);
            Assert.Equal(employee.Id, result.First().EmployeeId);
        }

        [Fact]
        public async Task GetBranchByEmployeeShiftId_WithValidId_ReturnsBranch()
        {
            var shift = await _context.EmployeeShifts.FirstAsync();
            var branch = await _repository.GetBranchByEmployeeShiftId(shift.Id);

            Assert.NotNull(branch);
            Assert.Equal("Branch A", branch.Name);
        }

        [Fact]
        public async Task GetBranchByEmployeeShiftId_WithInvalidId_ReturnsNull()
        {
            var result = await _repository.GetBranchByEmployeeShiftId(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEmployeeShiftById_WithValidId_ReturnsShift()
        {
            var shift = await _context.EmployeeShifts.FirstAsync();
            var result = await _repository.GetEmployeeShiftById(shift.Id);

            Assert.NotNull(result);
            Assert.Equal(shift.Id, result.Id);
            Assert.NotNull(result.WorkShift);
        }

        [Fact]
        public async Task GetEmployeeShiftById_WithDeletedShift_ReturnsNull()
        {
            var deletedShift = new EmployeeShift
            {
                Id = Guid.NewGuid(),
                IsDeleted = true,
                Status = "UnitTest"
            };
            _context.EmployeeShifts.Add(deletedShift);
            await _context.SaveChangesAsync();

            var result = await _repository.GetEmployeeShiftById(deletedShift.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetIndividualAttendance_WithValidEmployeeId_ReturnsList()
        {
            var employee = await _context.Users.FirstAsync();
            var result = await _repository.GetIndividualAttendance(employee.Id);

            Assert.NotEmpty(result);
            Assert.All(result, r => Assert.Equal(employee.Id, r.EmployeeId));
        }

        [Fact]
        public async Task GetIndividualAttendance_WithInvalidEmployeeId_ReturnsEmptyList()
        {
            var result = await _repository.GetIndividualAttendance(Guid.NewGuid());
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIndividualAttendance_OnlyReturnsLastMonthData()
        {
            var employee = await _context.Users.FirstAsync();

            // Add old record (more than 1 month ago)
            _context.EmployeeShifts.Add(new EmployeeShift
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                WorkDate = DateTime.Now.AddMonths(-2),
                IsDeleted = false,
                Status = "UnitTest"
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetIndividualAttendance(employee.Id);

            Assert.All(result, r => Assert.True(r.WorkDate >= DateTime.Now.AddMonths(-1)));
        }

        [Fact]
        public async Task Save_WhenChangesSaved_ReturnsTrue()
        {
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                FullName = "New User"
            });

            var result = await _repository.Save();

            Assert.True(result);
        }

        [Fact]
        public async Task Save_WhenNoChanges_ReturnsFalse()
        {
            var result = await _repository.Save();
            Assert.False(result);
        }
    }
}
