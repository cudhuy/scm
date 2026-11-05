using Domain.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Constanst;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class VoucherRepository : IVoucherRepository
    {

        private readonly SCMDbContext _context;

        private readonly ILogger<VoucherRepository> _logger;

        public VoucherRepository(SCMDbContext context, ILogger<VoucherRepository> logger)
        {

            _context = context;
            _logger = logger;
        }
        public async Task<List<Voucher>> GetListVoucher()
        {
            _logger.LogInformation("Entered method GetListVoucher in VoucherRepository");

            try
            {
                var vouchers = await _context.Vouchers
                    .Where(v => !v.IsDeleted)
                    .Include(v => v.MembershipRanksVouchers)
                    .ThenInclude(mrv => mrv.MembershipRank) // Load MembershipRank
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} vouchers.", vouchers.Count);
                return vouchers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving vouchers.");
                throw;
            }
        }



        public async Task<bool> CreateVoucher(Voucher voucher, List<Guid> membershipRankIds)
        {
            _logger.LogInformation("Entered method CreateVoucher in VoucherRepository");

            try
            {
                voucher.CreatedAt = DateTime.Now;
                voucher.UpdatedAt = DateTime.Now;
                voucher.IsDeleted = false;
                voucher.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                voucher.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;

                await _context.Vouchers.AddAsync(voucher);
                await _context.SaveChangesAsync();

                if (membershipRankIds != null && membershipRankIds.Any())
                {
                    foreach (var rankId in membershipRankIds)
                    {
                        var existingVoucher = await _context.MembershipRanksVouchers
                            .FirstOrDefaultAsync(m => m.VoucherId == voucher.Id && m.MembershipRankId == rankId);

                        if (existingVoucher == null)
                        {
                            var membershipRankVoucher = new MembershipRanksVouchers
                            {
                                VoucherId = voucher.Id,
                                MembershipRankId = rankId
                            };
                            await _context.MembershipRanksVouchers.AddAsync(membershipRankVoucher);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Successfully added new voucher with code {VoucherCode}.", voucher.Code);
                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the voucher.");
                return false;
            }
        }


        public async Task<Voucher> GetVoucherByCode(string code)
        {
            return await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == code && !v.IsDeleted);
        }

        public async Task<bool> DeleteVoucher(Guid id)
        {
            _logger.LogInformation("Entered method DeleteVoucher in 'VoucherRepository'");

            try
            {
                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Id == id);
                if (voucher == null)
                {
                    _logger.LogWarning("Voucher with ID {Id} not found.'VoucherRepository'", id);
                    return false;
                }

                voucher.IsDeleted = true;
                _context.Vouchers.Update(voucher);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully marked voucher with ID {Id} as deleted. 'VoucherRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the voucher as deleted. 'VoucherRepository'");
                throw;
            }
        }

        public async Task<bool> UpdateVoucher(Guid id, Voucher updatedVoucher, List<Guid> membershipRankIds)
        {
            _logger.LogInformation("Entered method UpdateVoucher in 'VoucherRepository'");

            try
            {
                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Id == id);
                if (voucher == null)
                {
                    _logger.LogWarning("Voucher with ID {Id} not found to update. 'VoucherRepository'", id);
                    return false; 
                }

                voucher.NoOfVoucher = updatedVoucher.NoOfVoucher;
                voucher.Description = updatedVoucher.Description;
                voucher.DiscountValue = updatedVoucher.DiscountValue;
                voucher.DiscountType = updatedVoucher.DiscountType;
                voucher.MinOrder = updatedVoucher.MinOrder;
                voucher.StartDate = updatedVoucher.StartDate;
                voucher.ExpireDate = updatedVoucher.ExpireDate;
                voucher.UpdatedAt = DateTime.Now;

                _context.Vouchers.Update(voucher);
                await _context.SaveChangesAsync();

                var existingRanks = await _context.MembershipRanksVouchers
                    .Where(m => m.VoucherId == id)
                    .ToListAsync();

                var ranksToRemove = existingRanks
                    .Where(rank => !membershipRankIds.Contains(rank.MembershipRankId))
                    .ToList();
                if (ranksToRemove.Any())
                {
                    _context.MembershipRanksVouchers.RemoveRange(ranksToRemove);
                }

                var ranksToAdd = membershipRankIds
                    .Where(rankId => !existingRanks.Any(m => m.MembershipRankId == rankId))
                    .Select(rankId => new MembershipRanksVouchers
                    {
                        VoucherId = id,
                        MembershipRankId = rankId
                    })
                    .ToList();

                if (ranksToAdd.Any())
                {
                    await _context.MembershipRanksVouchers.AddRangeAsync(ranksToAdd);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated voucher with ID {Id}.", id);
                return true; // Successfully updated
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the voucher.");
                return false; // Failed to update
            }
        }


        public async Task<Voucher> GetVoucherById(Guid id)
        {
            return await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Id == id
                    && v.ExpireDate > DateTime.Now
                    && !v.IsDeleted
                    && v.noOfVoucherUsed < v.NoOfVoucher);
        }

        public async Task<int> GetVoucherUsageCount(Guid voucherId)
        {
            _logger.LogInformation("Entered method GetVoucherUsageCount in 'VoucherRepository'");

            try
            {
                var usageCount = await _context.VoucherUsages
                    .CountAsync(vu => vu.VoucherId == voucherId && !vu.IsDeleted);

                _logger.LogInformation("Voucher with ID {VoucherId} has been used {UsageCount} times.", voucherId, usageCount);

                return usageCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting voucher usages.");
                throw;
            }
        }


        public async Task<List<VoucherUsage>> GetListVoucherUsed()
        {
            _logger.LogInformation("Entered method GetListVoucherUsed in 'VoucherRepository'");
            try
            {

                var voucherUsed = await _context.VoucherUsages
                            .Where(v => !v.IsDeleted)
                           .Include(v => v.Voucher)
                            .Include(v => v.User) 
                            .ToListAsync(); 
                
                _logger.LogInformation("Successfully retrieved {Count} voucherUseds.'VoucherRepository'", voucherUsed.Count);
                return voucherUsed;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while retrieving voucherUseds .'VoucherRepository'");
                throw;
            }
        }

        // user 
        public async Task<User> GetUserById(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Ranks) 
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<IEnumerable<MembershipRanksVouchers>> GetMembershipRankVouchers(Guid membershipRankId)
        {
            return await _context.MembershipRanksVouchers
                .Where(mr => mr.MembershipRankId == membershipRankId && !mr.IsDeleted && !mr.Voucher.IsDeleted)
                .Include(mr => mr.Voucher)
                .ToListAsync();
        }


    }
}
