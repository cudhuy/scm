using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Services.Interfaces;

namespace UserManagementTest.UnitTests.Controllers
{
    public class EmployeeShiftsControllerTests
    {
        private readonly Mock<IEmployeeShiftsService> _serviceMock;
        private readonly EmployeeShiftsController _controller;

        public EmployeeShiftsControllerTests()
        {
            _serviceMock = new Mock<IEmployeeShiftsService>();
            _controller = new EmployeeShiftsController(_serviceMock.Object);
        }

        [Fact]
        public async Task AddEmployeeShift_ShouldReturnOk_WhenSuccess()
        {
            var input = new CreateEmployeeShiftDto();
            _serviceMock.Setup(s => s.AddEmployeeShift(input)).ReturnsAsync(true);

            var result = await _controller.AddEmployeeShift(input);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Thêm ca làm việc thành công");
        }

        [Fact]
        public async Task AddEmployeeShift_ShouldReturnBadRequest_WhenFailed()
        {
            var input = new CreateEmployeeShiftDto();
            _serviceMock.Setup(s => s.AddEmployeeShift(input)).ReturnsAsync(false);

            var result = await _controller.AddEmployeeShift(input);

            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badResult.Value.Should().Be("Thêm ca làm việc thất bại");
        }

        [Fact]
        public async Task GetStaffs_ShouldReturnOk_WithStaffList()
        {
            var branchId = Guid.NewGuid();
            var staffs = new List<StaffInShift>();
            _serviceMock.Setup(s => s.GetStaffs(branchId)).ReturnsAsync(staffs);

            var result = await _controller.GetStaffs(branchId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(staffs);
        }

        [Fact]
        public async Task GetStaffSchedulesLastMonth_ShouldReturnOk()
        {
            var branchId = Guid.NewGuid();
            var schedules = new List<StaffScheduleResponse>();
            _serviceMock.Setup(s => s.GetStaffSchedulesLastMonth(branchId)).ReturnsAsync(schedules);

            var result = await _controller.GetStaffSchedulesLastMonth(branchId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(schedules);
        }

        [Fact]
        public async Task GetStaffScheduleByDate_ShouldReturnOk()
        {
            var input = new StaffScheduleParameter();
            var schedules = new StaffScheduleResponse();
            _serviceMock.Setup(s => s.GetStaffScheduleByDate(input)).ReturnsAsync(schedules);

            var result = await _controller.GetStaffScheduleByDate(input);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(schedules);
        }

        [Fact]
        public async Task DeleteEmployeeShift_ShouldReturnOk_WhenSuccess()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteEmployeeShift(id)).ReturnsAsync(true);

            var result = await _controller.DeleteEmployeeShift(id);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Xóa thành công!");
        }

        [Fact]
        public async Task DeleteEmployeeShift_ShouldReturnBadRequest_WhenFail()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteEmployeeShift(id)).ReturnsAsync(false);

            var result = await _controller.DeleteEmployeeShift(id);

            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badResult.Value.Should().Be("Xóa thất bại!");
        }
    }
}

