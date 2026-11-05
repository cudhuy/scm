
namespace Branch.API.Services.Interfaces
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Domain.Entities.Branch>> GetListBranch();
        Task<bool> CreateBranch(Domain.Entities.Branch branch);
        Task<Domain.Entities.Branch> GetBranchById(Guid id);
        Task<bool> UpdateBranch(Guid id, Domain.Entities.Branch UpdatedBranch);
        Task<bool> DeleteBranch(Guid id);
    }
}
