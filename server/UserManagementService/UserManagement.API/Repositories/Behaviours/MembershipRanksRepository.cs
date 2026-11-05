using AutoMapper;
using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Dtos;
using UserManagement.API.Dtos.MembershipRank;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class MembershipRanksRepository : IMembershipRanksRepository
    {
        private readonly ILogger<MembershipRanksRepository> _logger;
        private readonly SCMDbContext _context;
        private readonly IMapper _mapper;

        public MembershipRanksRepository(ILogger<MembershipRanksRepository> logger, SCMDbContext context, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public Task<bool> CreateMembershipRank(SaveMembershipRankDto memberShipRankDto)
        {
            _logger.LogInformation("Enter CreateMembershipRank method");
            var membershipRank = new MembershipRanks
            {
                Name = memberShipRankDto.Name,
                Description = memberShipRankDto.Description,
                CreatedAt = DateTime.Now,
                CreatedBy = AppConstants.SYSTEM_ROLE_GUID,
                UpdatedAt = null,
                UpdatedBy = null,
                IsDeleted = false
            };
            _context.MembershipRanks.Add(membershipRank);
            _logger.LogInformation("Leave CreateMembershipRank method");
            return Save();
        }

        public async Task<bool> DeleteMembershipRank(Guid id)
        {
            _logger.LogInformation("Enter DeleteMembershipRank method");
            var membershipRank = await _context.MembershipRanks.FirstOrDefaultAsync(m => m.Id == id);
            if (membershipRank == null) return await Task.FromResult(false);

            if (membershipRank.IsDeleted == true) membershipRank.IsDeleted = false;
            else membershipRank.IsDeleted = true;
            //_context.MembershipRanks.Update(membershipRank);
            _logger.LogInformation("Leave DeleteMembershipRank method");
            return await Save();
        }

        public async Task<List<MemberShipRankDto>> GetMembershipRanks()
        {
            List<MembershipRanks> membershipRanks = await _context.MembershipRanks.AsNoTracking().ToListAsync();
            List<MemberShipRankDto> memberShipRankDtos = _mapper.Map<List<MemberShipRankDto>>(membershipRanks);
            return memberShipRankDtos;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateMembershipRank(Guid id, SaveMembershipRankDto memberShipRankDto)
        {
            _logger.LogInformation("Enter UpdateMembershipRank method");
            var membershipRank = await _context.MembershipRanks.FirstOrDefaultAsync(m => m.Id == id);
            if (membershipRank == null) return await Task.FromResult(false);
            membershipRank.Name = memberShipRankDto.Name;
            membershipRank.Description = memberShipRankDto.Description;
            membershipRank.UpdatedAt = DateTime.Now;
            membershipRank.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;
            //_context.MembershipRanks.Update(membershipRank);
            _logger.LogInformation("Leave UpdateMembershipRank method");
            return await Save();
        }

        public async Task<List<MembershipRanks>> GetMembershipRanksByIds(List<Guid> membershipRankIds)
        {
            _logger.LogInformation("Entered GetMembershipRanksByIds method with {Count} IDs ' MembershipRanksRepository'", membershipRankIds.Count);
            try
            {
                if (membershipRankIds == null || membershipRankIds.Count == 0)
                {
                    _logger.LogWarning("The provided list of MembershipRank IDs is empty or null.");
                    return new List<MembershipRanks>();
                }

                var membershipRanks = await _context.MembershipRanks
                                                     .Where(m => membershipRankIds.Contains(m.Id))
                                                     .ToListAsync();

                if (membershipRanks.Count == 0)
                {
                    _logger.LogWarning("No membership ranks found for the provided IDs'MembershipRanksRepository'");
                }
                else
                {
                    _logger.LogInformation("{Count} membership ranks found for the provided IDs  ' MembershipRanksRepository'", membershipRanks.Count);
                }

                return membershipRanks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving membership ranks. ' MembershipRanksRepository'");
                throw;
            }
        }

        public Task<MemberShipRankReportDto> GetReport()
        {
            var report = new MemberShipRankReportDto
            {
                TotalCustomer = _context.Users.Where(u => u.RoleId == AppConstants.USER_ROLE_GUID).Count(),
                TotalCustomerRankRegular = _context.Users.Count(m => m.IsDeleted == false && m.RoleId == AppConstants.USER_ROLE_GUID && m.RankId == AppConstants.MEMBERSHIP_REGULARS_GUID),
                TotalCustomerStar = _context.Users.Count(m => m.IsDeleted == false && m.RoleId == AppConstants.USER_ROLE_GUID && m.RankId == AppConstants.MEMBERSHIP_STARS_GUID),
                TotalCustomerVip = _context.Users.Count(m => m.IsDeleted == false && m.RoleId == AppConstants.USER_ROLE_GUID && m.RankId == AppConstants.MEMBERSHIP_VIPS_GUID),
            };


            return Task.FromResult(report);
        }

        //auto update rank for customer
        public async Task<List<User>> GetUsersWithOrdersAsync()
        {
            return await _context.Users
                .Where(u => _context.Orders.Any(o => o.CustomerId == u.Id && o.Status == 2))
                .ToListAsync();
        }
        public async Task<decimal> GetTotalAmountByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == userId && o.Status == 2)
                .SumAsync(o => o.TotalAmount);
        }
        public async Task<MembershipRanks?> GetHighestEligibleRankAsync(decimal totalAmount)
        {
            return await _context.MembershipRanks
                .Where(r => totalAmount >= r.RequiredAmount)
                .OrderByDescending(r => r.RequiredAmount)
                .FirstOrDefaultAsync();
        }
        public async Task UpdateUserRankAsync(Guid userId, Guid rankId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RankId = rankId;
                await _context.SaveChangesAsync();
            }
        }


        public async Task<string?> GetUserEmailByIdAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId && !u.IsDeleted)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();
        }
    }
}
