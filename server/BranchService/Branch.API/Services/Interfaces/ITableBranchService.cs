using Branch.API.Dtos.TableBranch;

namespace Branch.API.Services.Interfaces
{
    public interface ITableBranchService
    {
        Task<bool> CreateTable(CreateTableDto createTableDto);
        Task<IEnumerable<TableDto>> GetAllTablesAsync(Guid? branchId, bool? isOccupied, string? tableNumber);
        Task<bool> ToggleTableStatus(Guid tableId);
        Task<bool> DeleteBranch(Guid id);
        Task<bool> UpdateTable(Guid tableId, UpdateTableDto updateTableDto);
        Task<TableResponse> GetTablesAsync(int page, int pageSize, Guid? branchId, bool? isOccupied, string? tableNumber);
        Task<bool?> ToggleTableDeleted(Guid id);
        Task<TableNotDeletedResponse> GetTablesNotDeleted(int page, int pageSize, Guid? branchId);
        Task<OfflineTableBranchInfoDto> GetBranchInfoById(Guid tableId);
    }
}
