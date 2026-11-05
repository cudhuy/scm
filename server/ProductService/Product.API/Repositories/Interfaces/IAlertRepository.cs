
namespace Product.API.Repositories.Interfaces
{
    public interface IAlertRepository
    {
        Task<string?> GetBranchManagerEmail(string inventoryName);
    }
}
