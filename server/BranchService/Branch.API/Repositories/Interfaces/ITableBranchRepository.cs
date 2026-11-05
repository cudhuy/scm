using Branch.API.Dtos.TableBranch;
using Domain.Entities;

namespace Branch.API.Services.Interfaces
{
    public interface ITableBranchRepository
    {
        Task<Domain.Entities.Branch> GetBranchById(Guid branchId);
        Task<BranchTable> GetByBranchAndTableNumber(Guid branchId, string tableNumber);
        Task<bool> AddTable(BranchTable table);
        Task<bool> UpdateTable(BranchTable table);
        Task<BranchTable?> GetTableByIdAsync(Guid tableId);
        Task<bool> DeleteTable(Guid id);
        Task<bool> UpdateStatusTableAsync(BranchTable table);
        Task<List<BranchTable>> GetByBranchAndTableNumberToUpdate(Guid branchId, string tableNumber, Guid tableId);
        Task<(List<BranchTable> Items, bool HasMore)> GetTablesForPage(int page, int pageSize, Guid? branchId, bool? isOccupied, string? tableNumber);
        Task<bool?> ToggleTableDeleted(Guid id);
        Task<(List<BranchTable> Items, bool HasMore)> GetTablesNotDeleted(int page, int pageSize, Guid? branchId);
        Task<IEnumerable<BranchTable>> GetAllTables(Guid? branchId, bool? isOccupied, string? tableNumber);
        Task<OfflineTableBranchInfoDto> GetBranchInfoById(Guid tableId);
    }
}
