using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Services.Interfaces;

namespace UserManagementTest.UnitTests.Controllers
{
    public class EmployeeAttendanceControllerTests
    {
        private readonly Mock<IEmployeeAttendanceService> _employeeAttendanceServiceMock;
        private readonly EmployeeAttendanceController _controller;

        public EmployeeAttendanceControllerTests()
        {
            _employeeAttendanceServiceMock = new Mock<IEmployeeAttendanceService>();
            _controller = new EmployeeAttendanceController(_employeeAttendanceServiceMock.Object);

            // Mock HttpContext để có thể kiểm tra claims
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        // ---- TEST CHECK-IN ----
        [Fact]
        public async Task CheckIn_ShouldReturnOk_WhenCheckInSuccessful()
        {
            // Arrange
            var employeeShiftId = Guid.NewGuid();
            var userId = Guid.NewGuid(); // Tạo ID giả lập cho user

            var input = new CheckInParameterDto { EmployeeShiftId = employeeShiftId };

            // Setup mock để đảm bảo userId khớp với người dùng hiện tại
            _employeeAttendanceServiceMock.Setup(s => s.GetEmployeeIdByEmployeeShift(employeeShiftId))
                .ReturnsAsync(userId);
            _employeeAttendanceServiceMock.Setup(s => s.CheckIn(input, It.IsAny<HttpContext>()))
                .ReturnsAsync(new AttendanceResponseDto { Result = true, Message = "Check-in successful" });

            // Setup HttpContext để giả lập User đang đăng nhập
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = await _controller.CheckIn(input);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Check-in successful");
        }


        [Fact]
        public async Task CheckIn_ShouldReturnBadRequest_WhenEmployeeNotFound()
        {
            // Arrange
            var input = new CheckInParameterDto { EmployeeShiftId = Guid.NewGuid() };
            _employeeAttendanceServiceMock.Setup(s => s.GetEmployeeIdByEmployeeShift(It.IsAny<Guid>()))
                .ReturnsAsync(Guid.Empty);

            // Act
            var result = await _controller.CheckIn(input);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Ca làm việc không hợp lệ hoặc không tìm thấy nhân viên!");
        }

        [Fact]
        public async Task CheckIn_ShouldReturnForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var input = new CheckInParameterDto { EmployeeShiftId = Guid.NewGuid() };
            var anotherUserId = Guid.NewGuid();

            _employeeAttendanceServiceMock.Setup(s => s.GetEmployeeIdByEmployeeShift(It.IsAny<Guid>()))
                .ReturnsAsync(anotherUserId);

            // Act
            var result = await _controller.CheckIn(input);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        // ---- TEST CHECK-OUT ----
        [Fact]
        public async Task CheckOut_ShouldReturnOk_WhenCheckOutSuccessful()
        {
            // Arrange
            var employeeShiftId = Guid.NewGuid();
            var input = new CheckOutParameterDto { EmployeeShiftId = employeeShiftId };
            var userId = Guid.NewGuid(); // Giả lập UserId hợp lệ

            _employeeAttendanceServiceMock.Setup(s => s.GetEmployeeIdByEmployeeShift(employeeShiftId))
                .ReturnsAsync(userId);
            _employeeAttendanceServiceMock.Setup(s => s.CheckOut(input, It.IsAny<HttpContext>()))
                .ReturnsAsync(new AttendanceResponseDto { Result = true, Message = "Check-out successful" });

            // Setup HttpContext giả lập User đang đăng nhập với ID = userId
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = await _controller.CheckOut(input);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Check-out successful");
        }


        [Fact]
        public async Task CheckOut_ShouldReturnBadRequest_WhenEmployeeNotFound()
        {
            // Arrange
            var input = new CheckOutParameterDto { EmployeeShiftId = Guid.NewGuid() };
            _employeeAttendanceServiceMock.Setup(s => s.GetEmployeeIdByEmployeeShift(It.IsAny<Guid>()))
                .ReturnsAsync(Guid.Empty);

            // Act
            var result = await _controller.CheckOut(input);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Ca làm việc không hợp lệ hoặc không tìm thấy nhân viên!");
        }

        [Fact]
        public async Task CheckOut_ShouldReturnForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var input = new CheckOutParameterDto { EmployeeShiftId = Guid.NewGuid() };
            var anotherUserId = Guid.NewGuid();

            _employeeAttendanceServiceMock.Setup(s => s.GetEmployeeIdByEmployeeShift(It.IsAny<Guid>()))
                .ReturnsAsync(anotherUserId);

            // Act
            var result = await _controller.CheckOut(input);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        // ---- TEST GET INDIVIDUAL ATTENDANCE ----
        [Fact]
        public async Task GetIndividualAttendance_ShouldReturnOk_WhenDataExists()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var attendanceData = new List<ScheduleDetail>();

            _employeeAttendanceServiceMock.Setup(s => s.GetIndividualAttendance(employeeId))
                .ReturnsAsync(attendanceData);

            // Setup HttpContext giả lập User đang đăng nhập với ID = employeeId
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, employeeId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = await _controller.GetIndividualAttendance(employeeId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(attendanceData);
        }


        [Fact]
        public async Task GetIndividualAttendance_ShouldReturnForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var anotherEmployeeId = Guid.NewGuid();

            // Act
            var result = await _controller.GetIndividualAttendance(anotherEmployeeId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }
    }
}
