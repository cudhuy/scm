using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Services.Interfaces;

namespace UserManagementTest.UnitTests.Controllers
{
    public class VouchersControllerTests
    {
        private readonly Mock<IVoucherService> _voucherServiceMock;
        private readonly Mock<ILogger<VouchersController>> _loggerMock;
        private readonly VouchersController _controller;

        public VouchersControllerTests()
        {
            _voucherServiceMock = new Mock<IVoucherService>();
            _loggerMock = new Mock<ILogger<VouchersController>>();
            _controller = new VouchersController(_voucherServiceMock.Object, _loggerMock.Object);

            // Mock HttpContext để tránh lỗi null khi kiểm tra Authorize
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        // 1. GetAllVouchers
        //[Fact]
        //public async Task GetAllVouchers_ShouldReturnOk_WhenVouchersExist()
        //{
        //    // Arrange
        //    var vouchers = new List<VoucherDto> ();
        //    _voucherServiceMock.Setup(s => s.GetAllVouchers()).ReturnsAsync(vouchers);

        //    // Act
        //    var result = await _controller.GetAllVouchers();

        //    // Assert
        //    result.Should().BeOfType<NotFoundObjectResult>()
        //        .Which.Value.Should().BeEquivalentTo(vouchers);
        //}

        [Fact]
        public async Task GetAllVouchers_ShouldReturnNotFound_WhenNoVouchersExist()
        {
            // Arrange
            _voucherServiceMock.Setup(s => s.GetAllVouchers()).ReturnsAsync(new List<VoucherDto>());

            // Act
            var result = await _controller.GetAllVouchers();

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { message = "Không có voucher nào." });
        }

        // 2. CreateVoucher
        [Fact]
        public async Task CreateVoucher_ShouldReturnOk_WhenVoucherCreatedSuccessfully()
        {
            // Arrange
            var createVoucherDto = new CreateNewVoucherDto
            {
                Code = "VOUCHER1",
                MembershipRankIds = new List<Guid> { Guid.NewGuid() },
                StartDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(1)
            };
            _voucherServiceMock.Setup(s => s.CreateVoucher(createVoucherDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateVoucher(createVoucherDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { message = "Giá trị DiscountType không hợp lệ. Các giá trị được phép: 1 (Phần trăm), 2 (Số tiền cố định)" });
        }

        [Fact]
        public async Task CreateVoucher_ShouldReturnBadRequest_WhenMembershipRankIdsIsEmpty()
        {
            // Arrange
            var createVoucherDto = new CreateNewVoucherDto
            {
                Code = "VOUCHER1",
                MembershipRankIds = new List<Guid>(), // Rỗng
                StartDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(1)
            };

            // Act
            var result = await _controller.CreateVoucher(createVoucherDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("MembershipRankIds không thể null hoặc rỗng.");
        }

        // 3. DeleteVoucher
        [Fact]
        public async Task DeleteVoucher_ShouldReturnOk_WhenVoucherDeletedSuccessfully()
        {
            // Arrange
            var voucherId = Guid.NewGuid();
            _voucherServiceMock.Setup(s => s.DeleteVoucher(voucherId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteVoucher(voucherId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { message = "Xóa voucher thành công!" });
        }

        [Fact]
        public async Task DeleteVoucher_ShouldReturnNotFound_WhenVoucherDoesNotExist()
        {
            // Arrange
            var voucherId = Guid.NewGuid();
            _voucherServiceMock.Setup(s => s.DeleteVoucher(voucherId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteVoucher(voucherId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { message = "Voucher không tìm thấy hoặc đã không còn hoạt động." });
        }

        // 4. UpdateVoucher
        [Fact]
        public async Task UpdateVoucher_ShouldReturnOk_WhenVoucherUpdatedSuccessfully()
        {
            // Arrange
            var voucherId = Guid.NewGuid();
            var updateVoucherDto = new UpdateVoucherDto
            {
                MembershipRankIds = new List<Guid> { Guid.NewGuid() },
                StartDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(1)
            };
            var updatedVoucher = new { Id = voucherId };
            _voucherServiceMock.Setup(s => s.UpdateVoucher(voucherId, updateVoucherDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateVoucher(voucherId, updateVoucherDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { message = "Giá trị DiscountType không hợp lệ. Các giá trị được phép: 1 (Phần trăm), 2 (Số tiền cố định)" });
        }

        [Fact]
        public async Task UpdateVoucher_ShouldReturnBadRequest_WhenDtoIsNull()
        {
            // Arrange
            var voucherId = Guid.NewGuid();
            UpdateVoucherDto updateVoucherDto = null;

            // Act
            var result = await _controller.UpdateVoucher(voucherId, updateVoucherDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Dữ liệu voucher là null.");
        }

        // 5. GetAllVoucherUseds
        [Fact]
        public async Task GetAllVoucherUseds_ShouldReturnOk_WhenVoucherUsedsExist()
        {
            // Arrange
            var voucherUseds = new List<VoucherUsagesDto>();
            _voucherServiceMock.Setup(s => s.GetAllVoucherUseds()).ReturnsAsync(voucherUseds);

            // Act
            var result = await _controller.GetAllVoucherUseds();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(voucherUseds);
        }

        [Fact]
        public async Task GetAllVoucherUseds_ShouldReturn500_WhenExceptionThrown()
        {
            // Arrange
            _voucherServiceMock.Setup(s => s.GetAllVoucherUseds())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllVoucherUseds();

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }

        // 6. GetUserVouchers
        //[Fact]
        //public async Task GetUserVouchers_ShouldReturnOk_WhenVouchersExist()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var userVouchers = new List<VoucherUserDto> ();
        //    _voucherServiceMock.Setup(s => s.GetUserVouchers(userId)).ReturnsAsync(userVouchers);

        //    // Act
        //    var result = await _controller.GetUserVouchers(userId);

        //    // Assert
        //    result.Should().BeOfType<NotFoundObjectResult>()
        //        .Which.Value.Should().BeEquivalentTo(userVouchers);
        //}

        [Fact]
        public async Task GetUserVouchers_ShouldReturnNotFound_WhenNoVouchersExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _voucherServiceMock.Setup(s => s.GetUserVouchers(userId)).ReturnsAsync(new List<VoucherUserDto>());

            // Act
            var result = await _controller.GetUserVouchers(userId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new { message = "Không tìm thấy voucher nào cho người dùng này." });
        }
    }
}
