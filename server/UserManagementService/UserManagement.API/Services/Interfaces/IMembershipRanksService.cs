using UserManagement.API.Dtos;
using UserManagement.API.Dtos.MembershipRank;

namespace UserManagement.API.Services.Interfaces
{
    public interface IMembershipRanksService
    {
        Task<bool> CreateMembershipRank(SaveMembershipRankDto memberShipRankDto);
        Task<bool> DeleteMembershipRank(Guid id);
        Task<List<MemberShipRankDto>> GetMembershipRanks();
        Task<MemberShipRankReportDto> GetReport();
        Task<bool> UpdateMembershipRank(Guid id, SaveMembershipRankDto memberShipRankDto);
    }
}
