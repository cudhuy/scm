using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Dtos.MembershipRank;
using UserManagement.API.Dtos;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;

namespace UserManagementTest.UnitTests.Services
{
    public class MembershipRanksServiceTests
    {
        private readonly Mock<ILogger<MembershipRanksService>> _loggerMock;
        private readonly Mock<IMembershipRanksRepository> _repositoryMock;
        private readonly MembershipRanksService _service;

        public MembershipRanksServiceTests()
        {
            _loggerMock = new Mock<ILogger<MembershipRanksService>>();
            _repositoryMock = new Mock<IMembershipRanksRepository>();
            _service = new MembershipRanksService(_loggerMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task CreateMembershipRank_ShouldReturnTrue_WhenSuccessful()
        {
            var dto = new SaveMembershipRankDto();
            _repositoryMock.Setup(r => r.CreateMembershipRank(dto)).ReturnsAsync(true);

            var result = await _service.CreateMembershipRank(dto);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteMembershipRank_ShouldReturnTrue_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteMembershipRank(id)).ReturnsAsync(true);

            var result = await _service.DeleteMembershipRank(id);

            Assert.True(result);
        }

        [Fact]
        public async Task GetMembershipRanks_ShouldReturnList()
        {
            var list = new List<MemberShipRankDto>
        {
            new MemberShipRankDto { Id = Guid.NewGuid(), Name = "Gold" },
            new MemberShipRankDto { Id = Guid.NewGuid(), Name = "Silver" }
        };

            _repositoryMock.Setup(r => r.GetMembershipRanks()).ReturnsAsync(list);

            var result = await _service.GetMembershipRanks();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetReport_ShouldReturnReportDto()
        {
            var report = new MemberShipRankReportDto
            {
                TotalCustomer = 100
            };

            _repositoryMock.Setup(r => r.GetReport()).ReturnsAsync(report);

            var result = await _service.GetReport();

            Assert.Equal(100, result.TotalCustomer);
        }

        [Fact]
        public async Task UpdateMembershipRank_ShouldReturnTrue_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var dto = new SaveMembershipRankDto();

            _repositoryMock.Setup(r => r.UpdateMembershipRank(id, dto)).ReturnsAsync(true);

            var result = await _service.UpdateMembershipRank(id, dto);

            Assert.True(result);
        }
    }
}
