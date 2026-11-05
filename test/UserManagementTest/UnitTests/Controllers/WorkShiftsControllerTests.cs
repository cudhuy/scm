using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    public class WorkShiftsControllerTests
    {
        private readonly Mock<IWorkShiftsService> _workShiftsServiceMock;
        private readonly WorkShiftsController _controller;

        public WorkShiftsControllerTests()
        {
            _workShiftsServiceMock = new Mock<IWorkShiftsService>();
            _controller = new WorkShiftsController(_workShiftsServiceMock.Object);

            // Mock HttpContext để tránh lỗi null khi kiểm tra Authorize
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        // 1. UpdateShift
        [Fact]
        public async Task UpdateShift_ShouldReturnOk_WhenUpdateSucceeds()
        {
            // Arrange
            var workShiftDto = new UpdateWorkShiftDto { /* Giả lập dữ liệu */ };
            _workShiftsServiceMock.Setup(s => s.UpdateShift(workShiftDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateShift(workShiftDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("Cập nhật ca làm việc thành công!");
        }

        [Fact]
        public async Task UpdateShift_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            var workShiftDto = new UpdateWorkShiftDto { /* Giả lập dữ liệu */ };
            _workShiftsServiceMock.Setup(s => s.UpdateShift(workShiftDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateShift(workShiftDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Cập nhật ca làm việc thất bại.");
        }

        // 2. GetShifts
        [Fact]
        public async Task GetShifts_ShouldReturnOk_WhenShiftsExist()
        {
            // Arrange
            var shifts = new List<WorkShiftResponseDto>();
            _workShiftsServiceMock.Setup(s => s.GetShifts()).ReturnsAsync(shifts);

            // Act
            var result = await _controller.GetShifts();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(shifts);
        }

        [Fact]
        public async Task GetShifts_ShouldReturnOk_WhenNoShiftsExist()
        {
            // Arrange
            var shifts = new List<WorkShiftResponseDto>(); // Danh sách rỗng
            _workShiftsServiceMock.Setup(s => s.GetShifts()).ReturnsAsync(shifts);

            // Act
            var result = await _controller.GetShifts();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(shifts);
        }
    }
}
