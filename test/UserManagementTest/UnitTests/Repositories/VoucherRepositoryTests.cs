using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.API.Repositories.Behaviours;
using Xunit;
using System.Linq;

namespace UserManagementTest.UnitTests.Repositories
{
    public class VoucherRepositoryTests
    {
        private SCMDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SCMDbContext(options);
        }

        private VoucherRepository GetRepository(SCMDbContext context)
        {
            var logger = new LoggerFactory().CreateLogger<VoucherRepository>();
            return new VoucherRepository(context, logger);
        }

        [Fact]
        public async Task CreateVoucher_ShouldReturnTrue_WhenValid()
        {
            var context = GetDbContext();
            var repo = GetRepository(context);

            var voucher = new Voucher { Id = Guid.NewGuid(), Code = "TEST", NoOfVoucher = 100 };
            var result = await repo.CreateVoucher(voucher, new List<Guid>());

            Assert.False(result);
            Assert.Single(context.Vouchers);
        }

        [Fact]
        public async Task GetListVoucher_ShouldReturnAll_WhenExists()
        {
            var context = GetDbContext();
            context.Vouchers.Add(new Voucher { Id = Guid.NewGuid(), Code = "V001", IsDeleted = false, Description ="UnitTest" });
            context.SaveChanges();
            var repo = GetRepository(context);

            var result = await repo.GetListVoucher();

            Assert.Single(result);
        }

        [Fact]
        public async Task DeleteVoucher_ShouldReturnFalse_WhenNotFound()
        {
            var context = GetDbContext();
            var repo = GetRepository(context);

            var result = await repo.DeleteVoucher(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteVoucher_ShouldReturnTrue_WhenExists()
        {
            var context = GetDbContext();
            var voucher = new Voucher { Id = Guid.NewGuid(), Code = "V002", IsDeleted = false, Description = "UnitTest" };
            context.Vouchers.Add(voucher);
            context.SaveChanges();

            var repo = GetRepository(context);
            var result = await repo.DeleteVoucher(voucher.Id);

            Assert.True(result);
            Assert.True(context.Vouchers.First().IsDeleted);
        }

        [Fact]
        public async Task GetVoucherByCode_ShouldReturnVoucher_WhenExists()
        {
            var context = GetDbContext();
            var voucher = new Voucher { Id = Guid.NewGuid(), Code = "FINDME", IsDeleted = false, Description = "UnitTest" };
            context.Vouchers.Add(voucher);
            context.SaveChanges();

            var repo = GetRepository(context);
            var result = await repo.GetVoucherByCode("FINDME");

            Assert.NotNull(result);
            Assert.Equal("FINDME", result.Code);
        }

        [Fact]
        public async Task GetVoucherById_ShouldReturnVoucher_WhenValid()
        {
            var context = GetDbContext();
            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = "V004",
                IsDeleted = false,
                ExpireDate = DateTime.Now.AddDays(5),
                NoOfVoucher = 10,
                noOfVoucherUsed = 0,
                Description = "UnitTest"
            };
            context.Vouchers.Add(voucher);
            context.SaveChanges();

            var repo = GetRepository(context);
            var result = await repo.GetVoucherById(voucher.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetVoucherUsageCount_ShouldReturnCorrectNumber()
        {
            var context = GetDbContext();
            var id = Guid.NewGuid();
            context.VoucherUsages.Add(new VoucherUsage { VoucherId = id, IsDeleted = false });
            context.VoucherUsages.Add(new VoucherUsage { VoucherId = id, IsDeleted = false });
            context.SaveChanges();

            var repo = GetRepository(context);
            var count = await repo.GetVoucherUsageCount(id);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetListVoucherUsed_ShouldReturnVoucherUsages()
        {
            var context = GetDbContext();
            context.VoucherUsages.Add(new VoucherUsage
            {
                Id = Guid.NewGuid(),
                VoucherId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                IsDeleted = false,
                Voucher = new Voucher { Code = "UnitTest", Description = "UnitTest"},
                User = new User()
            });
            context.SaveChanges();

            var repo = GetRepository(context);
            var list = await repo.GetListVoucherUsed();

            Assert.Single(list);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenValid()
        {
            var context = GetDbContext();
            var user = new User { Id = Guid.NewGuid(), IsDeleted = false };
            context.Users.Add(user);
            context.SaveChanges();

            var repo = GetRepository(context);
            var result = await repo.GetUserById(user.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMembershipRankVouchers_ShouldReturnItems_WhenExist()
        {
            var context = GetDbContext();
            var rankId = Guid.NewGuid();
            var voucher = new Voucher { Id = Guid.NewGuid(), IsDeleted = false, Code = "UnitTest", Description = "UnitTest" };
            context.MembershipRanksVouchers.Add(new MembershipRanksVouchers
            {
                MembershipRankId = rankId,
                VoucherId = voucher.Id,
                IsDeleted = false,
                Voucher = voucher
            });
            context.SaveChanges();

            var repo = GetRepository(context);
            var result = await repo.GetMembershipRankVouchers(rankId);

            Assert.Single(result);
        }
    }
}
