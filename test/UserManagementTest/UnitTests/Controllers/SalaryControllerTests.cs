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
    public class SalaryControllerTests
    {
        private readonly Mock<ISalaryService> _salaryServiceMock;
        private readonly SalaryController _controller;

        public SalaryControllerTests()
        {
            _salaryServiceMock = new Mock<ISalaryService>();
            _controller = new SalaryController(_salaryServiceMock.Object);
        }

        [Fact]
        public async Task GetSalaryList_ShouldReturnOk_WithSalaryList()
        {
            // Arrange
            var dto = new GetSalaryDto();
            var expected = new List<MonthlySalaryResponse> { };
            _salaryServiceMock.Setup(s => s.GetSalaryList(dto)).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetSalaryList(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task GetSalaryList_ShouldReturnOk_WithEmptyList()
        {
            var dto = new GetSalaryDto();
            _salaryServiceMock.Setup(s => s.GetSalaryList(dto)).ReturnsAsync(new List<MonthlySalaryResponse>());

            var result = await _controller.GetSalaryList(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<MonthlySalaryResponse>>(okResult.Value);
        }

        [Fact]
        public async Task ConfirmMonthlySalary_ShouldReturnOk_WhenSuccess()
        {
            var request = new List<CreateMonthlySalaryDto> { new CreateMonthlySalaryDto() };
            _salaryServiceMock.Setup(s => s.ConfirmMonthlySalary(request)).ReturnsAsync(true);

            var result = await _controller.ConfirmMonthlySalary(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Xác nhận thành công", okResult.Value);
        }

        [Fact]
        public async Task ConfirmMonthlySalary_ShouldReturnBadRequest_WhenFail()
        {
            var request = new List<CreateMonthlySalaryDto> { new CreateMonthlySalaryDto() };
            _salaryServiceMock.Setup(s => s.ConfirmMonthlySalary(request)).ReturnsAsync(false);

            var result = await _controller.ConfirmMonthlySalary(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Xác nhận thất bại", badRequest.Value);
        }

        [Fact]
        public async Task SendSalarySlip_ShouldReturnOk_WithTrue()
        {
            var dto = new SendSalarySlipDto();
            _salaryServiceMock.Setup(s => s.SendSalarySlip(dto)).ReturnsAsync(true);

            var result = await _controller.SendSalarySlip(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value!);
        }

        [Fact]
        public async Task SendSalarySlip_ShouldReturnOk_WithFalse()
        {
            var dto = new SendSalarySlipDto();
            _salaryServiceMock.Setup(s => s.SendSalarySlip(dto)).ReturnsAsync(false);

            var result = await _controller.SendSalarySlip(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.False((bool)okResult.Value!);
        }
    }
}
