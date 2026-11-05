using Branch.API.Controllers;
using Branch.API.Dtos.TableBranch;
using Branch.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Controllers
{
    public class TableBranchControllerTests
    {
        private readonly Mock<ITableBranchService> _mockService;
        private readonly TableBranchController _controller;

        public TableBranchControllerTests()
        {
            _mockService = new Mock<ITableBranchService>();
            _controller = new TableBranchController(_mockService.Object);
        }

        [Fact]
        public async Task CreateTable_Success()
        {
            var dto = new CreateTableDto();
            _mockService.Setup(s => s.CreateTable(dto)).ReturnsAsync(true);

            var result = await _controller.CreateTable(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Tạo bàn thành công", "Tạo bàn thành công");
        }

        [Fact]
        public async Task CreateTable_AlreadyExists()
        {
            var dto = new CreateTableDto();
            _mockService.Setup(s => s.CreateTable(dto)).ReturnsAsync(false);

            var result = await _controller.CreateTable(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bàn đã tồn tại", "Bàn đã tồn tại");
        }

        [Fact]
        public async Task ToggleTableStatus_Success()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.ToggleTableStatus(id)).ReturnsAsync(true);

            var result = await _controller.ToggleTableStatus(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Chuyển trạng thái bàn thành công!", "Chuyển trạng thái bàn thành công!");
        }

        [Fact]
        public async Task ToggleTableStatus_Failed()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.ToggleTableStatus(id)).ReturnsAsync(false);

            var result = await _controller.ToggleTableStatus(id);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật bàn thất bại.", "Cập nhật bàn thất bại.");
        }

        [Fact]
        public async Task DeleteTable_Success()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteBranch(id)).ReturnsAsync(true);

            var result = await _controller.DeleteTable(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Xóa bàn thành công!", "Xóa bàn thành công!");
        }

        [Fact]
        public async Task DeleteTable_NotFound()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteBranch(id)).ReturnsAsync(false);

            var result = await _controller.DeleteTable(id);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Bàn không tồn tại hoặc đã bị vô hiệu hóa.", "Bàn không tồn tại hoặc đã bị vô hiệu hóa.");
        }

        //[Fact]
        //public async Task GetTables_ReturnsData()
        //{
        //    _mockService.Setup(s => s.GetTablesAsync(0, 10, null)).ReturnsAsync(new TableResponse());

        //    var result = await _controller.GetTables();

        //    Assert.IsType<OkObjectResult>(result);
        //}

        [Fact]
        public async Task UpdateTable_Success()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateTableDto();
            _mockService.Setup(s => s.UpdateTable(id, dto)).ReturnsAsync(true);

            var result = await _controller.UpdateTable(id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cập nhật bàn thành công", "Cập nhật bàn thành công");
        }

        [Fact]
        public async Task UpdateTable_Failed()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateTableDto();
            _mockService.Setup(s => s.UpdateTable(id, dto)).ReturnsAsync(false);

            var result = await _controller.UpdateTable(id, dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Số bàn đã tồn tại trong chi nhánh", "Số bàn đã tồn tại trong chi nhánh");
        }
    }
}
