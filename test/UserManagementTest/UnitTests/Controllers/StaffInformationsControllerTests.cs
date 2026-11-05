using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Dtos.Staff;
using UserManagement.API.Services.Interfaces;

namespace UserManagementTest.UnitTests.Controllers
{
    public class StaffInformationsControllerTests
    {
        private readonly Mock<IStaffInformationService> _staffServiceMock;
        private readonly Mock<ILogger<VouchersController>> _loggerMock;
        private readonly StaffInformationsController _controller;

        public StaffInformationsControllerTests()
        {
            _staffServiceMock = new Mock<IStaffInformationService>();
            _loggerMock = new Mock<ILogger<VouchersController>>();
            _controller = new StaffInformationsController(_staffServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllStaff_ShouldReturnOkResult()
        {
            _staffServiceMock.Setup(s => s.GetAllStaffs(1, 10, "Test", Guid.NewGuid())).ReturnsAsync(new GetStaffsResponseDto());

            var result = await _controller.GetAllStaff();

            Assert.IsType<OkObjectResult>(result);
        }

        //[Fact]
        //public async Task DeleteStaff_ShouldReturnNotFound_WhenStaffNotExists()
        //{
        //    _staffServiceMock.Setup(s => s.DeleteStaff(It.IsAny<Guid>())).ReturnsAsync(false);

        //    var result = await _controller.DeleteStaff(Guid.NewGuid());

        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        //    Assert.Contains("không tồn tại", notFoundResult.Value.ToString());
        //}

        //[Fact]
        //public async Task DeleteStaff_ShouldReturnOk_WhenDeleted()
        //{
        //    _staffServiceMock.Setup(s => s.DeleteStaff(It.IsAny<Guid>())).ReturnsAsync(true);

        //    var result = await _controller.DeleteStaff(Guid.NewGuid());

        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    Assert.Contains("Xóa nhân viên thành công", okResult.Value.ToString());
        //}

        [Fact]
        public async Task UpdateStaff_ShouldReturnNotFound_WhenUpdateFails()
        {
            var dto = new UpdateStaffDto();
            _staffServiceMock.Setup(s => s.UpdateStaff(dto)).ReturnsAsync(false);

            var result = await _controller.UpdateStaff(dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("cập nhật thất bại", notFound.Value.ToString());
        }

        [Fact]
        public async Task UpdateStaff_ShouldReturnOk_WhenSuccess()
        {
            var dto = new UpdateStaffDto();
            _staffServiceMock.Setup(s => s.UpdateStaff(dto)).ReturnsAsync(true);

            var result = await _controller.UpdateStaff(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Cập nhật nhân viên thành công", ok.Value.ToString());
        }

        [Fact]
        public async Task CreateStaff_ShouldReturnConflict_WhenEmailExists()
        {
            var dto = new CreateStaffDto();
            _staffServiceMock.Setup(s => s.CreateStaff(dto)).ReturnsAsync(false);

            var result = await _controller.CreateStaff(dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Contains("đã tồn tại", conflict.Value.ToString());
        }

        [Fact]
        public async Task CreateStaff_ShouldReturnOk_WhenSuccess()
        {
            var dto = new CreateStaffDto();
            _staffServiceMock.Setup(s => s.CreateStaff(dto)).ReturnsAsync(true);

            var result = await _controller.CreateStaff(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Tạo nhân viên thành công", ok.Value.ToString());
        }

        [Fact]
        public async Task GetDetailStaff_ShouldReturnNotFound_WhenNull()
        {
            _staffServiceMock.Setup(s => s.GetDetailStaffAsync(It.IsAny<Guid>())).ReturnsAsync((StaffDetailDto)null);

            var result = await _controller.GetDetailStaff(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetDetailStaff_ShouldReturnOk_WhenExists()
        {
            _staffServiceMock.Setup(s => s.GetDetailStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new StaffDetailDto());

            var result = await _controller.GetDetailStaff(Guid.NewGuid());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetStaffProfile_ShouldReturnNotFound_WhenNotExists()
        {
            _staffServiceMock.Setup(s => s.GetStaffProfileAsync(It.IsAny<Guid>())).ReturnsAsync((StaffProfileDto)null);

            var result = await _controller.GetStaffProfile(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetStaffProfile_ShouldReturnOk_WhenExists()
        {
            _staffServiceMock.Setup(s => s.GetStaffProfileAsync(It.IsAny<Guid>())).ReturnsAsync(new StaffProfileDto());

            var result = await _controller.GetStaffProfile(Guid.NewGuid());

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
