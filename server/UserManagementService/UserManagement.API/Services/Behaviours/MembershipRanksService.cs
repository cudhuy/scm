using UserManagement.API.Dtos;
using UserManagement.API.Dtos.MembershipRank;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class MembershipRanksService : IMembershipRanksService
    {
        private readonly ILogger<MembershipRanksService> _logger;
        private readonly IMembershipRanksRepository _membershipRanksRepository;

        public MembershipRanksService(ILogger<MembershipRanksService> logger, IMembershipRanksRepository membershipRanksRepository)
        {
            _logger = logger;
            _membershipRanksRepository = membershipRanksRepository;
        }

        public async Task<bool> CreateMembershipRank(SaveMembershipRankDto memberShipRankDto)
        {
            _logger.LogInformation("Enter CreateMembershipRank method");
            bool result = await _membershipRanksRepository.CreateMembershipRank(memberShipRankDto);
            _logger.LogInformation("Leave CreateMembershipRank method");
            return result;
        }

        public async Task<bool> DeleteMembershipRank(Guid id)
        {
            _logger.LogInformation("Enter DeleteMembershipRank method");
            bool result = await _membershipRanksRepository.DeleteMembershipRank(id);
            _logger.LogInformation("Leave DeleteMembershipRank method");
            return result;
        }

        public async Task<List<MemberShipRankDto>> GetMembershipRanks()
        {
            _logger.LogInformation("Enter GetMembershipRanks method");
            var result = await _membershipRanksRepository.GetMembershipRanks();
            _logger.LogInformation("Leave GetMembershipRanks method");
            return result;
        }

        public async Task<MemberShipRankReportDto> GetReport()
        {
            return await _membershipRanksRepository.GetReport();
        }

        public async Task<bool> UpdateMembershipRank(Guid id, SaveMembershipRankDto memberShipRankDto)
        {
            _logger.LogInformation("Enter UpdateMembershipRank method");
            bool result = await _membershipRanksRepository.UpdateMembershipRank(id, memberShipRankDto);
            _logger.LogInformation("Leave UpdateMembershipRank method");
            return result;
        }
    }
}
