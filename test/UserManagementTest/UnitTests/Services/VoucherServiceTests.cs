using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Dtos;
using UserManagement.API.Dtos.MembershipRank;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;
using Xunit;

namespace UserManagementTest.UnitTests.Services
{
    public class VoucherServiceTests
    {
        private readonly Mock<ILogger<VoucherService>> _loggerMock = new();
        private readonly Mock<IVoucherRepository> _voucherRepoMock = new();
        private readonly Mock<IMembershipRanksRepository> _membershipRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly VoucherService _service;

        public VoucherServiceTests()
        {
            _service = new VoucherService(
                _loggerMock.Object,
                _voucherRepoMock.Object,
                _mapperMock.Object,
                _membershipRepoMock.Object
            );
        }

        [Fact]
        public async Task GetAllVouchers_ReturnsVoucherDtos()
        {
            // Arrange
            var vouchers = new List<Voucher>
            {
                new Voucher
                {
                    Id = Guid.NewGuid(),
                    Code = "ABC123",
                    DiscountValue = 10,
                    DiscountType = (int)VoucherEnum.EDiscountType.Percentage,
                    MembershipRanksVouchers = new List<MembershipRanksVouchers>
                    {
                        new MembershipRanksVouchers
                        {
                            MembershipRank = new MembershipRanks { Id = Guid.NewGuid(), Name = "Gold", Description = "Gold Desc" }
                        }
                    }
                }
            };

            _voucherRepoMock.Setup(r => r.GetListVoucher()).ReturnsAsync(vouchers);

            // Act
            var result = await _service.GetAllVouchers();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllVoucherUseds_ReturnsUsages()
        {
            var voucherUseds = new List<VoucherUsage>
            {
                new VoucherUsage
                {
                    Voucher = new Voucher { Id = Guid.NewGuid(), Description = "Discount", DiscountValue = 10, DiscountType = 0, StartDate = DateTime.Now, ExpireDate = DateTime.Now.AddDays(7) },
                    User = new User { Id = Guid.NewGuid(), FullName = "Test User", Email = "test@mail.com" },
                    UsedAt = DateTime.Now
                }
            };

            _voucherRepoMock.Setup(r => r.GetListVoucherUsed()).ReturnsAsync(voucherUseds);

            var result = await _service.GetAllVoucherUseds();

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateVoucher_Success()
        {
            var dto = new CreateNewVoucherDto
            {
                Code = "ABC123",
                MembershipRankIds = new List<Guid> { Guid.NewGuid() }
            };

            _voucherRepoMock.Setup(r => r.GetVoucherByCode(dto.Code)).ReturnsAsync((Voucher)null);
            _membershipRepoMock.Setup(r => r.GetMembershipRanksByIds(dto.MembershipRankIds))
                               .ReturnsAsync(new List<MembershipRanks> { new() });
            _mapperMock.Setup(m => m.Map<Voucher>(dto)).Returns(new Voucher());

            _voucherRepoMock.Setup(r => r.CreateVoucher(It.IsAny<Voucher>(), dto.MembershipRankIds))
                            .ReturnsAsync(true);

            var result = await _service.CreateVoucher(dto);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteVoucher_Success()
        {
            var id = Guid.NewGuid();
            _voucherRepoMock.Setup(r => r.DeleteVoucher(id)).ReturnsAsync(true);

            var result = await _service.DeleteVoucher(id);
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateVoucher_Success()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateVoucherDto { MembershipRankIds = new List<Guid> { Guid.NewGuid() } };

            _voucherRepoMock.Setup(r => r.GetVoucherById(id)).ReturnsAsync(new Voucher { Id = id });
            _membershipRepoMock.Setup(r => r.GetMembershipRanksByIds(dto.MembershipRankIds))
                               .ReturnsAsync(new List<MembershipRanks> { new() });

            _mapperMock.Setup(m => m.Map<Voucher>(dto)).Returns(new Voucher { Id = id });
            _voucherRepoMock.Setup(r => r.UpdateVoucher(id, It.IsAny<Voucher>(), dto.MembershipRankIds))
                            .ReturnsAsync(true);

            var result = await _service.UpdateVoucher(id, dto);
            Assert.True(result);
        }

        [Fact]
        public async Task GetUserVouchers_ReturnsVouchers()
        {
            var userId = Guid.NewGuid();
            var rankId = Guid.NewGuid();
            _voucherRepoMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(new User
            {
                Id = userId,
                Ranks = new MembershipRanks { Id = rankId }
            });

            _voucherRepoMock.Setup(r => r.GetMembershipRankVouchers(rankId)).ReturnsAsync(new List<MembershipRanksVouchers>
            {
                new() { VoucherId = Guid.NewGuid() }
            });

            _voucherRepoMock.Setup(r => r.GetVoucherById(It.IsAny<Guid>())).ReturnsAsync(new Voucher
            {
                Id = Guid.NewGuid(),
                Code = "XYZ",
                Description = "Test",
                DiscountValue = 10,
                ExpireDate = DateTime.Now.AddDays(7),
                DiscountType = (int)VoucherEnum.EDiscountType.FixedAmount
            });

            var result = await _service.GetUserVouchers(userId);
            Assert.NotNull(result);
        }
    }
}
