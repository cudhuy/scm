using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;

namespace UserManagementTest.UnitTests.Services
{
    public class EmployeeShiftsServiceTests
    {
        private readonly Mock<ILogger<EmployeeShiftsService>> _loggerMock;
        private readonly Mock<IEmployeeShiftsRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EmployeeShiftsService _service;

        public EmployeeShiftsServiceTests()
        {
            _loggerMock = new Mock<ILogger<EmployeeShiftsService>>();
            _repositoryMock = new Mock<IEmployeeShiftsRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new EmployeeShiftsService(_loggerMock.Object, _repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AddEmployeeShift_ShouldReturnFalse_WhenWorkDateTooFar()
        {
            var input = new CreateEmployeeShiftDto
            {
                WorkDate = DateTime.Now.AddDays(8)
            };

            var result = await _service.AddEmployeeShift(input);

            Assert.False(result);
        }

        [Fact]
        public async Task AddEmployeeShift_ShouldReturnTrue_WhenValid()
        {
            var input = new CreateEmployeeShiftDto
            {
                WorkDate = DateTime.Now.AddDays(2)
            };

            var shiftEntity = new EmployeeShift();

            _mapperMock.Setup(m => m.Map<CreateEmployeeShiftDto, EmployeeShift>(input)).Returns(shiftEntity);
            _repositoryMock.Setup(r => r.AddEmployeeShift(shiftEntity)).ReturnsAsync(true);

            var result = await _service.AddEmployeeShift(input);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteEmployeeShift_ShouldReturnTrue_WhenFound()
        {
            var id = Guid.NewGuid();
            var shift = new EmployeeShift();

            _repositoryMock.Setup(r => r.GetEmployeeShiftById(id)).ReturnsAsync(shift);
            _repositoryMock.Setup(r => r.DeleteEmployeeShift(shift)).ReturnsAsync(true);

            var result = await _service.DeleteEmployeeShift(id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteEmployeeShift_ShouldReturnFalse_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetEmployeeShiftById(id)).ReturnsAsync((EmployeeShift)null);

            var result = await _service.DeleteEmployeeShift(id);

            Assert.False(result);
        }

        [Fact]
        public async Task GetStaffs_ShouldReturnMappedList()
        {
            var branchId = Guid.NewGuid();
            var users = new List<User> { new User(), new User() };
            var staffDtos = new List<StaffInShift> { new StaffInShift(), new StaffInShift() };

            _repositoryMock.Setup(r => r.GetStaffs(branchId)).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<User>, List<StaffInShift>>(users)).Returns(staffDtos);

            var result = await _service.GetStaffs(branchId);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetStaffScheduleByDate_ShouldReturnMappedResult()
        {
            var input = new StaffScheduleParameter
            {
                BranchId = Guid.NewGuid(),
                WorkDate = DateTime.Today
            };

            var shiftEntities = new List<EmployeeShift> { new EmployeeShift() };
            var mappedDetails = new List<ScheduleDetail> { new ScheduleDetail() };

            _repositoryMock.Setup(r => r.GetStaffScheduleByDate(input)).ReturnsAsync(shiftEntities);
            _mapperMock.Setup(m => m.Map<List<ScheduleDetail>>(shiftEntities)).Returns(mappedDetails);

            var result = await _service.GetStaffScheduleByDate(input);

            Assert.Equal(input.BranchId, result.BranchId);
            Assert.Single(result.ScheduleDetail);
        }

        [Fact]
        public async Task GetStaffSchedulesLastMonth_ShouldGroupAndMapCorrectly()
        {
            var branchId = Guid.NewGuid();
            var date = DateTime.Today;

            var shiftEntities = new List<EmployeeShift>
        {
            new EmployeeShift { WorkDate = date },
            new EmployeeShift { WorkDate = date }
        };

            var mapped = new List<ScheduleDetail>
        {
            new ScheduleDetail { WorkDate = date },
            new ScheduleDetail { WorkDate = date }
        };

            _repositoryMock.Setup(r => r.GetStaffSchedulesLastMonth(branchId)).ReturnsAsync(shiftEntities);
            _mapperMock.Setup(m => m.Map<List<ScheduleDetail>>(shiftEntities)).Returns(mapped);

            var result = await _service.GetStaffSchedulesLastMonth(branchId);

            Assert.Single(result);
            Assert.Equal(date, result[0].WorkDate);
            Assert.Equal(2, result[0].ScheduleDetail.Count);
        }
    }
}
