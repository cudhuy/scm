using AutoMapper;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Dtos;
using UserManagement.API.Repositories.Behaviours;

namespace UserManagementTest.UnitTests.Repositories
{
    public class MembershipRanksRepositoryTests
    {
        private readonly SCMDbContext _context;
        private readonly MembershipRanksRepository _repository;
        private readonly IMapper _mapper;

        public MembershipRanksRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SCMDbContext(options);
            var mockLogger = new Mock<ILogger<MembershipRanksRepository>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MembershipRanks, MemberShipRankDto>();
            });
            _mapper = config.CreateMapper();

            _repository = new MembershipRanksRepository(mockLogger.Object, _context, _mapper);
        }

        [Fact]
        public async Task CreateMembershipRank_ShouldAddRank()
        {
            var dto = new SaveMembershipRankDto { Name = "Gold", Description = "Gold Rank" };

            var result = await _repository.CreateMembershipRank(dto);

            Assert.True(result);
            Assert.Single(_context.MembershipRanks);
        }

        [Fact]
        public async Task DeleteMembershipRank_ShouldMarkAsDeleted()
        {
            var rank = new MembershipRanks { Name = "Silver", IsDeleted = false, Avatar = "UnitTest", Description = "UnitTest" };
            _context.MembershipRanks.Add(rank);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteMembershipRank(rank.Id);

            Assert.True(result);
            Assert.True(_context.MembershipRanks.First().IsDeleted);
        }

        [Fact]
        public async Task GetMembershipRanks_ShouldReturnOnlyNotDeleted()
        {
            _context.MembershipRanks.Add(new MembershipRanks {Name = "A", IsDeleted = false, Avatar = "UnitTest", Description = "UnitTest" });
            _context.MembershipRanks.Add(new MembershipRanks {Name = "B", IsDeleted = true, Avatar = "UnitTest", Description = "UnitTest" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetMembershipRanks();

            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateMembershipRank_ShouldModifyProperties()
        {
            var rank = new MembershipRanks { Name = "Basic", Description = "Old", Avatar = "old.png" };
            _context.MembershipRanks.Add(rank);
            await _context.SaveChangesAsync();

            var dto = new SaveMembershipRankDto { Name = "Updated", Description = "New" };
            var result = await _repository.UpdateMembershipRank(rank.Id, dto);

            var updated = await _context.MembershipRanks.FindAsync(rank.Id);

            Assert.True(result);
            Assert.Equal("Updated", updated.Name);
        }

        [Fact]
        public async Task GetMembershipRanksByIds_ShouldReturnCorrectItems()
        {
            var rank1 = new MembershipRanks { Name = "A", Avatar = "UnitTest", Description = "UnitTest" };
            var rank2 = new MembershipRanks { Name = "B", Avatar = "UnitTest", Description = "UnitTest" };
            _context.MembershipRanks.AddRange(rank1, rank2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetMembershipRanksByIds(new List<Guid> { rank1.Id });

            Assert.Single(result);
            Assert.Equal("A", result.First().Name);
        }

        [Fact]
        public async Task GetReport_ShouldReturnCorrectCounts()
        {
            _context.Users.Add(new User { RoleId = Domain.Constanst.AppConstants.USER_ROLE_GUID, RankId = Domain.Constanst.AppConstants.MEMBERSHIP_REGULARS_GUID });
            _context.Users.Add(new User { RoleId = Domain.Constanst.AppConstants.USER_ROLE_GUID, RankId = Domain.Constanst.AppConstants.MEMBERSHIP_STARS_GUID });
            await _context.SaveChangesAsync();

            var report = await _repository.GetReport();

            Assert.Equal(2, report.TotalCustomer);
            Assert.Equal(1, report.TotalCustomerRankRegular);
            Assert.Equal(1, report.TotalCustomerStar);
        }

        [Fact]
        public async Task GetUsersWithOrdersAsync_ShouldReturnMatchingUsers()
        {
            var user = new User();
            var order = new Order { CustomerId = user.Id, Status = 2, TotalAmount = 100, OrderCode = "UnitTest", ShippingAddress ="UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest" };
            _context.Users.Add(user);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var result = await _repository.GetUsersWithOrdersAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTotalAmountByUserIdAsync_ShouldReturnSum()
        {
            var userId = Guid.NewGuid();
            _context.Orders.AddRange(
                new Order { CustomerId = userId, Status = 2, TotalAmount = 50, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone ="UnitTest", ShippingAddress = "UnitTest" },
                new Order { CustomerId = userId, Status = 2, TotalAmount = 70, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "UnitTest" }
            );
            await _context.SaveChangesAsync();

            var total = await _repository.GetTotalAmountByUserIdAsync(userId);

            Assert.Equal(120, total);
        }

        [Fact]
        public async Task GetHighestEligibleRankAsync_ShouldReturnHighestRank()
        {
            _context.MembershipRanks.AddRange(
                new MembershipRanks { Name = "Bronze", RequiredAmount = 50, Avatar = "UnitTest", Description = "UnitTest" },
                new MembershipRanks { Name = "Gold", RequiredAmount = 100, Avatar = "UnitTest", Description = "UnitTest" }
            );
            await _context.SaveChangesAsync();

            var rank = await _repository.GetHighestEligibleRankAsync(120);

            Assert.Equal("Gold", rank.Name);
        }

        [Fact]
        public async Task UpdateUserRankAsync_ShouldUpdateRank()
        {
            var user = new User();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var newRankId = Guid.NewGuid();
            await _repository.UpdateUserRankAsync(user.Id, newRankId);

            var updated = await _context.Users.FindAsync(user.Id);
            Assert.Equal(newRankId, updated.RankId);
        }

        [Fact]
        public async Task GetUserEmailByIdAsync_ShouldReturnEmail()
        {
            var user = new User { Email = "test@example.com", IsDeleted = false };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var email = await _repository.GetUserEmailByIdAsync(user.Id);

            Assert.Equal("test@example.com", email);
        }
    }
}
