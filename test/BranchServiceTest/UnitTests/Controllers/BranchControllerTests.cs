using Branch.API.Controllers;
using Branch.API.Dtos.Branch;
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
    public class BranchControllerTests
    {
        private readonly Mock<IBranchService> _mockBranchService;
        private readonly BranchController _controller;

        public BranchControllerTests()
        {
            _mockBranchService = new Mock<IBranchService>();
            _controller = new BranchController(_mockBranchService.Object);
        }

        [Fact]
        public async Task GetAllBranch_ReturnsOk_WhenBranchesExist()
        {
            var branches = new List<BranchDto> { new BranchDto(), new BranchDto() };
            _mockBranchService.Setup(s => s.GetAllBranch()).ReturnsAsync(branches);

            var result = await _controller.GetAllBranch();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetAllBranch_ReturnsNotFound_WhenNoBranches()
        {
            _mockBranchService.Setup(s => s.GetAllBranch()).ReturnsAsync(new List<BranchDto>());

            var result = await _controller.GetAllBranch();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task CreateBranch_ReturnsCreated_WhenValid()
        {
            var dto = new CreateBranchDto();
            _mockBranchService.Setup(s => s.CreateBranch(dto)).ReturnsAsync(true);

            var result = await _controller.CreateBranch(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        [Fact]
        public async Task CreateBranch_ReturnsBadRequest_WhenDtoIsNull()
        {
            var result = await _controller.CreateBranch(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task UpdateBranch_ReturnsOk_WhenUpdated()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateBranchDto();
            _mockBranchService.Setup(s => s.UpdateBranch(id, dto)).ReturnsAsync(true);

            var result = await _controller.UpdateBranch(id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task UpdateBranch_ReturnsNotFound_WhenUpdateFails()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateBranchDto();
            _mockBranchService.Setup(s => s.UpdateBranch(id, dto)).ReturnsAsync(false);

            var result = await _controller.UpdateBranch(id, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task GetBranchById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            _mockBranchService.Setup(s => s.GetBranchById(id)).ReturnsAsync(new BranchDetailDto());

            var result = await _controller.GetBranchById(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task GetBranchById_ReturnsNotFound_WhenNotFound()
        {
            _mockBranchService.Setup(s => s.GetBranchById(It.IsAny<Guid>())).ReturnsAsync((BranchDetailDto)null);

            var result = await _controller.GetBranchById(Guid.NewGuid());

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task DeleteBranch_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _mockBranchService.Setup(s => s.DeleteBranch(id)).ReturnsAsync(true);

            var result = await _controller.DeleteBranch(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task DeleteBranch_ReturnsNotFound_WhenNotDeleted()
        {
            var id = Guid.NewGuid();
            _mockBranchService.Setup(s => s.DeleteBranch(id)).ReturnsAsync(false);

            var result = await _controller.DeleteBranch(id);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task GetAllBranchBase_ReturnsOk_WhenFound()
        {
            var branches = new List<BranchBaseDto> { new BranchBaseDto() };
            _mockBranchService.Setup(s => s.GetAllBranchBase()).ReturnsAsync(branches);

            var result = await _controller.GetAllBranchBase();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task GetAllBranchBase_ReturnsNotFound_WhenEmpty()
        {
            _mockBranchService.Setup(s => s.GetAllBranchBase()).ReturnsAsync(new List<BranchBaseDto>());

            var result = await _controller.GetAllBranchBase();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }
}
