using UserManagement.API.Dtos;
using Domain.Entities;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<List<Voucher>> GetListVoucher();
        Task<bool> CreateVoucher(Voucher voucher, List<Guid> membershipRankIds);
        Task<Voucher> GetVoucherByCode(string code);
        Task<int> GetVoucherUsageCount(Guid idVoucher);
        Task<Voucher> GetVoucherById(Guid id);
        Task<bool> DeleteVoucher(Guid id);
        Task<bool> UpdateVoucher(Guid id, Voucher updatedVoucher, List<Guid> membershipRankIds); 
        Task<List<VoucherUsage>> GetListVoucherUsed();
        //user 
        Task<User> GetUserById(Guid userId);
        Task<IEnumerable<MembershipRanksVouchers>> GetMembershipRankVouchers(Guid membershipRankId);
    }
}
