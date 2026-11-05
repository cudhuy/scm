using UserManagement.API.Dtos;
using Domain.Entities;
using UserManagement.API.Dtos.MembershipRank;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface IMembershipRanksRepository
    {
        Task<bool> CreateMembershipRank(SaveMembershipRankDto memberShipRankDto);
        Task<bool> DeleteMembershipRank(Guid id);
        Task<List<MemberShipRankDto>> GetMembershipRanks();
        Task<bool> Save();
        Task<bool> UpdateMembershipRank(Guid id, SaveMembershipRankDto memberShipRankDto);
        Task<List<MembershipRanks>> GetMembershipRanksByIds(List<Guid> membershipRankIds);
        Task<MemberShipRankReportDto> GetReport();

        //auto update membership rank
        Task<List<User>> GetUsersWithOrdersAsync();
        Task<decimal> GetTotalAmountByUserIdAsync(Guid userId);
        Task<MembershipRanks?> GetHighestEligibleRankAsync(decimal totalAmount);
        Task UpdateUserRankAsync(Guid userId, Guid rankId);
        Task<string?> GetUserEmailByIdAsync(Guid userId); 
    }
}
