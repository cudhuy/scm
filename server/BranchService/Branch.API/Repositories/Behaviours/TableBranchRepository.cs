using Branch.API.Dtos.TableBranch;
using Branch.API.Services.Interfaces;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Services.Behaviours
{
    public class TableBranchRepository : ITableBranchRepository
    {
        private readonly SCMDbContext _context;
        private readonly ILogger<TableBranchRepository> _logger;

        public TableBranchRepository(SCMDbContext context, ILogger<TableBranchRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Domain.Entities.Branch> GetBranchById(Guid branchId)
        {
            return await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId && !b.IsDeleted);
        }
        public async Task<BranchTable> GetByBranchAndTableNumber(Guid branchId, string tableNumber)
        {
            return await _context.BranchTables
                .FirstOrDefaultAsync(t => t.BranchId == branchId && t.TableNumber == tableNumber && !t.IsDeleted);
        }
        public async Task<bool> AddTable(BranchTable table)
        {
            await _context.BranchTables.AddAsync(table);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateTable(BranchTable table)
        {
            _context.BranchTables.Update(table);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<BranchTable?> GetTableByIdAsync(Guid tableId)
        {
            try
            {
                return await _context.BranchTables
                    .Where(b => b.Id == tableId && !b.IsDeleted)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving table with ID {TableId}.", tableId);
                throw;
            }
        }
        public async Task<List<BranchTable>> GetByBranchAndTableNumberToUpdate(Guid branchId, string tableNumber, Guid tableId)
        {
            return await _context.BranchTables
                .Where(t =>
                    t.BranchId == branchId &&
                    t.TableNumber == tableNumber &&
                    !t.IsDeleted &&
                    t.Id != tableId)
                .ToListAsync();
        }



        public async Task<bool> UpdateStatusTableAsync(BranchTable table)
        {
            try
            {
                _context.BranchTables.Update(table);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating table ID {TableId}.", table.Id);
                return false;
            }
        }

        public async Task<bool> DeleteTable(Guid id)
        {
            _logger.LogInformation("Entered method DeleteTable in 'TableBranchRepository'");

            try
            {
                var table = await _context.BranchTables.FirstOrDefaultAsync(v => v.Id == id);
                if (table == null)
                {
                    _logger.LogWarning("Table with ID {Id} not found.'TableBranchRepository'", id);
                    return false;
                }

                table.IsDeleted = true;
                _context.BranchTables.Update(table);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully marked table with ID {Id} as deleted. 'TableBranchRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the table as deleted. 'TableBranchRepository'");
                throw;
            }
        }
        public async Task<IEnumerable<BranchTable>> GetAllTables(Guid? branchId, bool? isOccupied, string? tableNumber)
        {
            _logger.LogInformation("Entered method GetAllTables in TableBranchRepository");

            try
            {
                var query = _context.BranchTables.AsQueryable();

                if (branchId.HasValue)
                    query = query.Where(t => t.BranchId == branchId.Value);

                if (isOccupied.HasValue)
                    query = query.Where(t => t.IsOccupied == isOccupied.Value);

                if (!string.IsNullOrWhiteSpace(tableNumber))
                    query = query.Where(t => t.TableNumber.Contains(tableNumber));

                var result = await query.ToListAsync();
                _logger.LogInformation("Retrieved {Count} tables.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tables.");
                throw;
            }
        }

        public async Task<(List<BranchTable> Items, bool HasMore)> GetTablesForPage(int page, int pageSize, Guid? branchId, bool? isOccupied, string? tableNumber)
        {
            _logger.LogInformation("Fetching tables with filters. Page: {Page}, PageSize: {PageSize}, BranchId: {BranchId}, IsOccupied: {IsOccupied}, TableNumber: {TableNumber}",
                                    page, pageSize, branchId, isOccupied, tableNumber);

            try
            {
                var baseQuery = _context.BranchTables.AsQueryable();

                if (branchId.HasValue)
                {
                    baseQuery = baseQuery.Where(t => t.BranchId == branchId.Value);
                }

                if (isOccupied.HasValue)
                {
                    baseQuery = baseQuery.Where(t => t.IsOccupied == isOccupied.Value);
                }

                if (!string.IsNullOrWhiteSpace(tableNumber))
                {
                    baseQuery = baseQuery.Where(t => t.TableNumber.Contains(tableNumber));
                }

                var totalItems = await baseQuery.CountAsync();
                var hasMore = (page + 1) * pageSize < totalItems;

                var tables = await baseQuery
                    .OrderBy(t => t.TableNumber)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (tables, hasMore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tables from database.");
                throw;
            }
        }


        public async Task<(List<BranchTable> Items, bool HasMore)> GetTablesNotDeleted(int page, int pageSize, Guid? branchId)
        {
            _logger.LogInformation("Fetching tables with pagination. Page: {Page}, PageSize: {PageSize}, BranchId: {BranchId}", page, pageSize, branchId);

            try
            {
                var baseQuery = _context.BranchTables.Where(bt => !bt.IsDeleted).AsQueryable();

                if (branchId.HasValue)
                {
                    baseQuery = baseQuery.Where(t => t.BranchId == branchId.Value);
                }

                var totalItems = await baseQuery.CountAsync();
                var hasMore = (page + 1) * pageSize < totalItems;

                var tables = await baseQuery
                    .OrderBy(t => t.TableNumber)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (tables, hasMore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tables from database.");
                throw;
            }
        }
        public async Task<bool?> ToggleTableDeleted(Guid id)
        {
            _logger.LogInformation("Entered ToggleTableStatus in TableBranchRepository");

            try
            {
                var table = await _context.BranchTables.FirstOrDefaultAsync(t => t.Id == id);
                if (table == null)
                {
                    _logger.LogWarning("Table with ID {Id} not found.", id);
                    return null;
                }

                table.IsDeleted = !table.IsDeleted;
                _context.BranchTables.Update(table);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Toggled IsDeleted status for table ID {Id} to {Status}", id, table.IsDeleted);
                return table.IsDeleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while toggling IsDeleted for table.");
                throw;
            }
        }


        public async Task<OfflineTableBranchInfoDto> GetBranchInfoById(Guid tableId)
        {
            var info = await _context.BranchTables
                .Where(t => t.Id == tableId && !t.IsDeleted)
                .Select(t => new { t.Id, t.Branch.Name, t.TableNumber })
                .FirstOrDefaultAsync();

            return info != null
                ? new OfflineTableBranchInfoDto
                {
                    tableId = info.Id,
                    tableNumber = info.TableNumber,
                    branchName = info.Name
                } : null;
        }
    }
}
