using AutoMapper;
using Branch.API.Dtos.TableBranch;
using Branch.API.Services.Interfaces;
using Domain.Entities;

namespace Branch.API.Services.Behaviours
{
    public class TableBranchService : ITableBranchService
    {
        private readonly ITableBranchRepository _tableBranchRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TableBranchRepository> _logger;

        public TableBranchService(ITableBranchRepository tableBranchRepository, IMapper mapper, ILogger<TableBranchRepository> logger)
        {
            _tableBranchRepository = tableBranchRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CreateTable(CreateTableDto createTableDto)
        {
            _logger.LogInformation("Entering CreateTable method for BranchId: {BranchId}, TableNumber: {TableNumber}",
                createTableDto.BranchId, createTableDto.TableNumber);

            var branch = await _tableBranchRepository.GetBranchById(createTableDto.BranchId);
            if (branch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found.", createTableDto.BranchId);
                throw new KeyNotFoundException("Không tìm thấy chi nhánh");
            }

            var existingTable = await _tableBranchRepository.GetByBranchAndTableNumber(createTableDto.BranchId, createTableDto.TableNumber);
            if (existingTable != null)
            {
                _logger.LogWarning("Table with number {TableNumber} already exists in Branch {BranchId}.",
                    createTableDto.TableNumber, createTableDto.BranchId);
                return false;
            }

            var newTable = new BranchTable
            {
                TableNumber = createTableDto.TableNumber,
                BranchId = createTableDto.BranchId,
                IsOccupied = false
            };

            var result = await _tableBranchRepository.AddTable(newTable);
            if (result)
            {
                _logger.LogInformation("Successfully created Table {TableNumber} in Branch {BranchId}.",
                    createTableDto.TableNumber, createTableDto.BranchId);
                return true;
            }

            _logger.LogWarning("Failed to create Table {TableNumber} in Branch {BranchId}.",
                createTableDto.TableNumber, createTableDto.BranchId);
            return false;
        }

        public async Task<bool> ToggleTableStatus(Guid tableId)
        {
            try
            {
                _logger.LogInformation("Toggling status for Table ID: {TableId}", tableId);

                var table = await _tableBranchRepository.GetTableByIdAsync(tableId);
                if (table == null)
                {
                    _logger.LogWarning(" Table with ID {TableId} not found.", tableId);
                    throw new KeyNotFoundException($"Không tìm thấy bàn này ");
                }

                var hasActiveOrders = table.Orders.Any(o => o.Status == 0 || o.Status == 1 || o.Status == 4);
                if (hasActiveOrders && table.IsOccupied)
                {
                    _logger.LogWarning("Table ID {TableId} has active orders. Cannot mark as empty.", tableId);
                    throw new InvalidOperationException("Bàn này vẫn còn đơn hàng đang xử lý. Không thể đánh dấu trống bàn.");
                }


                table.IsOccupied = !table.IsOccupied;

                var result = await _tableBranchRepository.UpdateStatusTableAsync(table);
                if (!result)
                {
                    _logger.LogError("Failed to update table status for Table ID {TableId}.", tableId);
                    throw new Exception("Không cập nhật được trạng thái bảng.");
                }

                _logger.LogInformation(" Table status updated successfully for Table ID {TableId}.", tableId);
                return true;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "KeyNotFoundException: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while toggling table ID {TableId}.", tableId);
                throw new Exception("An unexpected error occurred while updating table status.");
            }
        }

        public async Task<bool> DeleteBranch(Guid id)
        {
            _logger.LogInformation("Entered method DeleteBranch in service for table ID {Id}'TableBranchService'", id);

            try
            {
                var existingTable = await _tableBranchRepository.GetTableByIdAsync(id);
                if (existingTable == null)
                {
                    _logger.LogWarning("Table with ID {Id} not found.", id);
                    return false;
                };



                var result = await _tableBranchRepository.DeleteTable(id);
                if (!result)
                {
                    _logger.LogWarning("Table with ID {Id} not found in service.'TableBranchService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully marked Branch with ID {Id} as deleted in service.'BranchService'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(DeleteBranch)}: {ex.Message}, StackTrace: {ex.StackTrace}'BranchService'");
                throw;
            }
        }



        public async Task<TableResponse> GetTablesAsync(int page, int pageSize, Guid? branchId, bool? isOccupied, string? tableNumber)
        {
            var tables = await _tableBranchRepository.GetTablesForPage(page, pageSize, branchId, isOccupied, tableNumber);

            var tableDtos = tables.Items.Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                BranchId = t.BranchId,
                IsOccupied = t.IsOccupied,
                IsDeleted = t.IsDeleted
            }).ToList();

            return new TableResponse
            {
                Items = tableDtos,
                NextPage = tables.HasMore ? page + 1 : null
            };
        }

        public async Task<IEnumerable<TableDto>> GetAllTablesAsync(Guid? branchId, bool? isOccupied, string? tableNumber)
        {
            _logger.LogInformation("Enter method GetAllTablesAsync.");

            try
            {
                var tables = await _tableBranchRepository.GetAllTables(branchId, isOccupied, tableNumber);

                if (tables == null || !tables.Any())
                {
                    _logger.LogInformation("No tables found.");
                    return Enumerable.Empty<TableDto>();
                }

                var tableDtos = tables.Select(t => new TableDto
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    BranchId = t.BranchId,
                    IsOccupied = t.IsOccupied,
                    IsDeleted = t.IsDeleted
                  
                });

                return tableDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service GetAllTablesAsync.");
                throw;
            }
        }



        public async Task<TableNotDeletedResponse> GetTablesNotDeleted(int page, int pageSize, Guid? branchId)
        {
            var tables = await _tableBranchRepository.GetTablesNotDeleted(page, pageSize, branchId);

            var tableDtos = tables.Items.Select(t => new TableNotDeletedDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                BranchId = t.BranchId,
                IsOccupied = t.IsOccupied,
            }).ToList();

            return new TableNotDeletedResponse
            {
                Items = tableDtos,
                NextPage = tables.HasMore ? page + 1 : null
            };
        }
        public async Task<bool> UpdateTable(Guid tableId, UpdateTableDto updateTableDto)
        {
            _logger.LogInformation("Entering UpdateTable method for TableId: {TableId}", tableId);

            var table = await _tableBranchRepository.GetTableByIdAsync(tableId);
            if (table == null)
            {
                _logger.LogWarning("Table with ID {TableId} not found.", tableId);
                throw new KeyNotFoundException("Không tìm thấy bàn.");
            }

            var existingTables = await _tableBranchRepository
             .GetByBranchAndTableNumberToUpdate(table.BranchId, updateTableDto.TableNumber, tableId);

            if (existingTables.Any())
            {
                _logger.LogWarning("Table number {TableNumber} already exists in Branch {BranchId}.",
                    updateTableDto.TableNumber, table.BranchId);
                return false;
            }

            table.TableNumber = updateTableDto.TableNumber;
            table.UpdatedAt = DateTime.Now;
            var result = await _tableBranchRepository.UpdateTable(table);
            if (result)
            {
                _logger.LogInformation("Successfully updated Table {TableNumber} in Branch {BranchId}.",
                    updateTableDto.TableNumber, table.BranchId);
                return true;
            }

            _logger.LogWarning("Failed to update Table {TableNumber} in Branch {BranchId}.",
                updateTableDto.TableNumber, table.BranchId);
            return false;
        }

        public async Task<bool?> ToggleTableDeleted(Guid id)
        {
            _logger.LogInformation("Entered ToggleTableStatus in TableBranchService for ID {Id}", id);

            try
            {
                var result = await _tableBranchRepository.ToggleTableDeleted(id);
                if (result == null)
                {
                    _logger.LogWarning("Table with ID {Id} not found in service.", id);
                    return null;
                }

                _logger.LogInformation("Toggled IsDeleted status for table ID {Id} to {Status}", id, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ToggleTableStatus: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }


        public Task<OfflineTableBranchInfoDto> GetBranchInfoById(Guid tableId)
        {
            return _tableBranchRepository.GetBranchInfoById(tableId);
        }
    }
}
