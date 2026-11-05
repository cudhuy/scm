using Branch.API.Dtos.Branch;

namespace Branch.API.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllBranch();
        Task<bool> CreateBranch(CreateBranchDto createBranchDto);
        Task<bool> UpdateBranch(Guid id, UpdateBranchDto updateBranchDto);
        Task<bool> DeleteBranch(Guid id);
        Task<BranchDetailDto> GetBranchById(Guid id);
        Task<IEnumerable<BranchBaseDto>> GetAllBranchBase();
    }
}
