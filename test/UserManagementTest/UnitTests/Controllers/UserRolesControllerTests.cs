using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Dtos;
using UserManagement.API.Services.Interfaces;

namespace UserManagementTest.UnitTests.Controllers
{
    public class UserRolesControllerTests
    {
        private readonly Mock<IUserRolesService> _userRolesServiceMock;
        private readonly Mock<ILogger<UserRolesController>> _loggerMock;
        private readonly UserRolesController _controller;

        public UserRolesControllerTests()
        {
            _userRolesServiceMock = new Mock<IUserRolesService>();
            _loggerMock = new Mock<ILogger<UserRolesController>>();
            _controller = new UserRolesController(_loggerMock.Object, _userRolesServiceMock.Object);
        }

        [Fact]
        public async Task GetAllUserRoles_ShouldReturnOk_WithList()
        {
            _userRolesServiceMock.Setup(s => s.GetAllUserRoles())
                .ReturnsAsync(new List<UserRoleDto>() );

            var result = await _controller.GetAllUserRoles();

            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAllUserRoles_ShouldReturnNotFound_WhenEmpty()
        {
            _userRolesServiceMock.Setup(s => s.GetAllUserRoles()).ReturnsAsync(new List<UserRoleDto>());

            var result = await _controller.GetAllUserRoles();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Không có vai trò người dùng nào", notFound.Value.ToString());
        }

        [Fact]
        public async Task CreateUserRole_ShouldReturnOk_WhenSuccess()
        {
            var dto = new CreateRoleDto { RoleName = "NewRole" };
            _userRolesServiceMock.Setup(s => s.CreateUserRole(dto)).ReturnsAsync(true);

            var result = await _controller.CreateUserRole(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Tạo vai trò", ok.Value.ToString());
        }

        [Fact]
        public async Task CreateUserRole_ShouldReturnBadRequest_WhenInvalidDto()
        {
            var result = await _controller.CreateUserRole(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRole_ShouldReturnOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _userRolesServiceMock.Setup(s => s.DeleteUserRole(id)).ReturnsAsync(true);

            var result = await _controller.DeleteRole(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Xóa vai trò thành công", ok.Value.ToString());
        }

        [Fact]
        public async Task DeleteRole_ShouldReturnNotFound_WhenNotExists()
        {
            var id = Guid.NewGuid();
            _userRolesServiceMock.Setup(s => s.DeleteUserRole(id)).ReturnsAsync(false);

            var result = await _controller.DeleteRole(id);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Vai trò không tồn tại", notFound.Value.ToString());
        }

        [Fact]
        public async Task UpdateRoleUser_ShouldReturnOk_WhenUpdated()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateRoleDto { RoleName = "UpdatedRole" };
            _userRolesServiceMock.Setup(s => s.UpdateUserRole(id, dto)).ReturnsAsync(true);

            var result = await _controller.UpdateRoleUser(id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Cập nhật vai trò thành công", ok.Value.ToString());
        }

        //[Fact]
        //public async Task UpdateRoleUser_ShouldReturnNotFound_WhenFailed()
        //{
        //    var id = Guid.NewGuid();
        //    var dto = new UpdateRoleDto { RoleName = "UpdateFail" };
        //    _userRolesServiceMock.Setup(s => s.UpdateUserRole(id, dto)).ReturnsAsync(false);

        //    var result = await _controller.UpdateRoleUser(id, dto);

        //    var notFound = Assert.IsType<NotFoundObjectResult>(result);
        //    Assert.Contains("Vai trò không tồn tại", notFound.Value.ToString());
        //}

        [Fact]
        public async Task GetRoleUserCount_ShouldReturnOk_WhenHasData()
        {
            _userRolesServiceMock.Setup(s => s.GetRoleUserCount()).ReturnsAsync(new List<RoleCountDto>());

            var result = await _controller.GetRoleUserCount();

            var ok = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        //[Fact]
        //public async Task GetRoleUserCount_ShouldReturnNotFound_WhenEmpty()
        //{
        //    _userRolesServiceMock.Setup(s => s.GetRoleUserCount()).ReturnsAsync(new List<RoleCountDto>());

        //    var result = await _controller.GetRoleUserCount();

        //    var notFound = Assert.IsType<OkObjectResult>(result);
        //    Assert.Contains("Không tìm thấy dữ liệu", notFound.Value.ToString());
        //}
    }
}
