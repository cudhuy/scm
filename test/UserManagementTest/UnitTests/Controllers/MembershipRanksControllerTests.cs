using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Dtos;
using UserManagement.API.Dtos.MembershipRank;
using UserManagement.API.Services.Interfaces;
using Xunit;

namespace UserManagementTest.UnitTests.Controllers
{
    public class MembershipRanksControllerTests
    {
        private readonly Mock<IMembershipRanksService> _membershipRanksServiceMock;
        private readonly MembershipRanksController _controller;

        public MembershipRanksControllerTests()
        {
            _membershipRanksServiceMock = new Mock<IMembershipRanksService>();
            _controller = new MembershipRanksController(_membershipRanksServiceMock.Object);
        }

        [Fact]
        public async Task GetMembershipRanks_ShouldReturnOk_WithData()
        {
            var mockData = new List<MemberShipRankDto> { new MemberShipRankDto { Id = Guid.NewGuid(), Name = "Gold" } };
            _membershipRanksServiceMock.Setup(s => s.GetMembershipRanks()).ReturnsAsync(mockData);

            var result = await _controller.GetMembershipRanks();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(mockData);
        }

        [Fact]
        public async Task CreateMembershipRank_ShouldReturnOk_WhenSuccessful()
        {
            var dto = new SaveMembershipRankDto { Name = "Silver"};
            _membershipRanksServiceMock.Setup(s => s.CreateMembershipRank(dto)).ReturnsAsync(true);

            var result = await _controller.CreateMembershipRank(dto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Tạo thành công!");
        }

        [Fact]
        public async Task CreateMembershipRank_ShouldReturnBadRequest_WhenFailed()
        {
            var dto = new SaveMembershipRankDto { Name = "Silver" };
            _membershipRanksServiceMock.Setup(s => s.CreateMembershipRank(dto)).ReturnsAsync(false);

            var result = await _controller.CreateMembershipRank(dto);

            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badResult.Value.Should().Be("Tạo hạng thành viên thất bại");
        }

        [Fact]
        public async Task SoftDeleteMembershipRank_ShouldReturnOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            _membershipRanksServiceMock.Setup(s => s.DeleteMembershipRank(id)).ReturnsAsync(true);

            var result = await _controller.SoftDeleteMembershipRank(id);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Xóa thành công!");
        }

        [Fact]
        public async Task UpdateMembershipRank_ShouldReturnOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var dto = new SaveMembershipRankDto { Name = "Platinum"};
            _membershipRanksServiceMock.Setup(s => s.UpdateMembershipRank(id, dto)).ReturnsAsync(true);

            var result = await _controller.UpdateMembershipRank(id, dto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Cập nhật thành công!");
        }

        [Fact]
        public async Task GetReport_ShouldReturnOk_WithReport()
        {
            var report = new MemberShipRankReportDto { TotalCustomer = 10 };
            _membershipRanksServiceMock.Setup(s => s.GetReport()).ReturnsAsync(report);

            var result = await _controller.GetReport();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(report);
        }

        [Fact]
        public async Task GetMembershipRanks_ShouldReturnOkWithList()
        {
            var membershipRanks = new List<MemberShipRankDto> { new() { Id = Guid.NewGuid(), Name = "Gold" } };
            _membershipRanksServiceMock.Setup(s => s.GetMembershipRanks()).ReturnsAsync(membershipRanks);

            var result = await _controller.GetMembershipRanks();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(membershipRanks);
        }

        [Fact]
        public async Task CreateMembershipRank_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.CreateMembershipRank(new SaveMembershipRankDto());

            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateMembershipRank_ShouldReturnBadRequest_WhenServiceFails()
        {
            var input = new SaveMembershipRankDto { Name = "Bronze" };
            _membershipRanksServiceMock.Setup(s => s.CreateMembershipRank(input)).ReturnsAsync(false);

            var result = await _controller.CreateMembershipRank(input);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SoftDeleteMembershipRank_ShouldReturnBadRequest_WhenFailed()
        {
            var id = Guid.NewGuid();
            _membershipRanksServiceMock.Setup(s => s.DeleteMembershipRank(id)).ReturnsAsync(false);

            var result = await _controller.SoftDeleteMembershipRank(id);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateMembershipRank_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.UpdateMembershipRank(Guid.NewGuid(), new SaveMembershipRankDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateMembershipRank_ShouldReturnBadRequest_WhenServiceFails()
        {
            var id = Guid.NewGuid();
            var input = new SaveMembershipRankDto { Name = "Platinum" };
            _membershipRanksServiceMock.Setup(s => s.UpdateMembershipRank(id, input)).ReturnsAsync(false);

            var result = await _controller.UpdateMembershipRank(id, input);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetReport_ShouldReturnOk_WithReportData()
        {
            var report = new MemberShipRankReportDto { TotalCustomer = 100};
            _membershipRanksServiceMock.Setup(s => s.GetReport()).ReturnsAsync(report);

            var result = await _controller.GetReport();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(report);
        }

    }
}
