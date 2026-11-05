using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;

namespace UserManagementTest.UnitTests.Services
{
    public class EmployeeAttendanceServiceTests
    {
        private readonly Mock<ILogger<EmployeeAttendanceService>> _loggerMock;
        private readonly Mock<IEmployeeAttendanceRepository> _attendanceRepoMock;
        private readonly Mock<IEmployeeShiftsRepository> _shiftRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EmployeeAttendanceService _service;

        public EmployeeAttendanceServiceTests()
        {
            _loggerMock = new Mock<ILogger<EmployeeAttendanceService>>();
            _attendanceRepoMock = new Mock<IEmployeeAttendanceRepository>();
            _shiftRepoMock = new Mock<IEmployeeShiftsRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new EmployeeAttendanceService(_loggerMock.Object, _attendanceRepoMock.Object, _mapperMock.Object, _shiftRepoMock.Object);
        }

        private static HttpContext CreateHttpContextWithIp(string ip)
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse(ip);
            return context;
        }

        [Fact]
        public async Task CheckIn_ShouldSucceed_WhenValidAndInTime()
        {
            // Arrange
            var employeeShiftId = Guid.NewGuid();
            var workDate = new DateTime(2025, 4, 10);
            var startTime = new TimeSpan(8, 0, 0);

            var employeeShift = new EmployeeShift
            {
                Id = employeeShiftId,
                WorkDate = workDate,
                WorkShift = new WorkShift { StartTime = startTime, GracePeriod = 15 },
                Status = nameof(EmployeeShiftEnum.EStatus.NotStarted)
            };

            _attendanceRepoMock.Setup(r => r.GetBranchByEmployeeShiftId(employeeShiftId))
                .ReturnsAsync(new Branch { IpAddress = "127.0.0.1" });

            _attendanceRepoMock.Setup(r => r.GetEmployeeShiftById(employeeShiftId))
                .ReturnsAsync(employeeShift);

            _attendanceRepoMock.Setup(r => r.Save()).ReturnsAsync(true);

            var httpContext = CreateHttpContextWithIp("127.0.0.1");

            var input = new CheckInParameterDto { EmployeeShiftId = employeeShiftId, StartAmount = 100 };

            // Act
            var result = await _service.CheckIn(input, httpContext);

            // Assert
            Assert.True(result.Result);
            Assert.Equal("Check in thành-công!", result.Message);
        }

        [Fact]
        public async Task CheckIn_ShouldFail_WhenTooEarly()
        {
            // Arrange
            var employeeShiftId = Guid.NewGuid();
            var workDate = new DateTime(2025, 4, 10);
            var startTime = new TimeSpan(8, 0, 0);

            var employeeShift = new EmployeeShift
            {
                Id = employeeShiftId,
                WorkDate = workDate,
                WorkShift = new WorkShift { StartTime = startTime, GracePeriod = 15 },
                Status = nameof(EmployeeShiftEnum.EStatus.NotStarted)
            };

            _attendanceRepoMock.Setup(r => r.GetBranchByEmployeeShiftId(employeeShiftId))
                .ReturnsAsync(new Branch { IpAddress = "127.0.0.1" });

            _attendanceRepoMock.Setup(r => r.GetEmployeeShiftById(employeeShiftId))
                .ReturnsAsync(employeeShift);

            var httpContext = CreateHttpContextWithIp("127.0.0.1");

            var input = new CheckInParameterDto { EmployeeShiftId = employeeShiftId, StartAmount = 100 };

            // Mock DateTime too early for check-in (override CheckInDate)
            employeeShift.CheckInDate = new DateTime(2025, 4, 10, 7, 15, 0); // 45 minutes before

            // Act
            var result = await _service.CheckIn(input, httpContext);

            // Assert
            Assert.False(result.Result);
            Assert.Equal("Check in thành-công!", result.Message);
        }

        [Fact]
        public async Task CheckOut_ShouldSucceed_WhenValidAndInTime()
        {
            var employeeShiftId = Guid.NewGuid();
            var workDate = new DateTime(2025, 4, 10);
            var endTime = new TimeSpan(12, 0, 0);

            var employeeShift = new EmployeeShift
            {
                Id = employeeShiftId,
                WorkDate = workDate,
                WorkShift = new WorkShift { EndTime = endTime, GracePeriod = 15 },
                Status = nameof(EmployeeShiftEnum.EStatus.Started)
            };

            _attendanceRepoMock.Setup(r => r.GetBranchByEmployeeShiftId(employeeShiftId))
                .ReturnsAsync(new Branch { IpAddress = "127.0.0.1" });

            _attendanceRepoMock.Setup(r => r.GetEmployeeShiftById(employeeShiftId))
                .ReturnsAsync(employeeShift);

            _attendanceRepoMock.Setup(r => r.Save()).ReturnsAsync(true);

            var httpContext = CreateHttpContextWithIp("127.0.0.1");

            var input = new CheckOutParameterDto { EmployeeShiftId = employeeShiftId, EndAmount = 200 };

            // Act
            var result = await _service.CheckOut(input, httpContext);

            // Assert
            Assert.True(result.Result);
            Assert.Equal("Check-out thành công!", result.Message);
        }
    }
}
