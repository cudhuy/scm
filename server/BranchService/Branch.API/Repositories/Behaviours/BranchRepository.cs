using Branch.API.Services.Interfaces;
using Domain.Constanst;
using Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Services.Behaviours
{
    public class BranchRepository : IBranchRepository
    {
        private readonly SCMDbContext _context;
        private readonly ILogger<BranchRepository> _logger;

        public BranchRepository(SCMDbContext context, ILogger<BranchRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Domain.Entities.Branch>> GetListBranch()
        {
            _logger.LogInformation("Entered method GetListBranch in BranchRepository");

            try
            {
                var branches = await _context.Branches
                    .Where(v => !v.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} Branch.", branches.Count);
                return branches;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while retrieving Branch.");
                throw new Exception("Database error occurred. Please try again later.");
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogError(invEx, "Invalid operation error while retrieving Branch.");
                throw new Exception("Invalid operation while fetching data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Branch.");
                throw;
            }
        }
        public async Task<bool> CreateBranch(Domain.Entities.Branch branch)
        {
            _logger.LogInformation("Entered method CreateBranch in BranchRepository");

            try
            {
                branch.IsActive = true;
                branch.CreatedAt = DateTime.Now;
                branch.UpdatedAt = DateTime.Now;
                branch.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                branch.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;
                branch.IsDeleted = false;
                branch.Type = "Sub";

                await _context.Branches.AddAsync(branch);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added new Branch with ID {Id}.", branch.Id);
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error occurred while adding the Branch.");
                throw new InvalidOperationException("Database update failed. Please try again later.", dbEx);
            }
            catch (ArgumentNullException argNullEx)
            {
                _logger.LogError(argNullEx, "Attempted to add a null branch.");
                throw new ArgumentException("Branch data is missing or invalid.", argNullEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the Branch.");
                return false;
            }
        }

        public async Task<Domain.Entities.Branch> GetBranchById(Guid id)
        {
            return await _context.Branches.FirstOrDefaultAsync(v => v.Id == id);

        }

        public async Task<bool> UpdateBranch(Guid id, Domain.Entities.Branch UpdatedBranch)
        {
            _logger.LogInformation("Entered method UpdateBranch in 'BranchRepository'");

            try
            {
                var branchs = await _context.Branches.FirstOrDefaultAsync(v => v.Id == id);
                if (branchs == null)
                {
                    _logger.LogWarning("Branch with ID {Id} not found to update. 'BranchRepository'", id);
                    return false;
                }

                branchs.Name = UpdatedBranch.Name;
                branchs.Address = UpdatedBranch.Address;
                branchs.Email = UpdatedBranch.Email;
                branchs.Phone = UpdatedBranch.Phone;
                branchs.UpdatedAt = DateTime.Now;

                _context.Branches.Update(branchs);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated Branch with ID {Id}.'BranchRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Branch.'BranchRepository'");
                return false;
            }
        }

        public async Task<bool> DeleteBranch(Guid id)
        {
            _logger.LogInformation("Entered method DeleteBranch in 'BranchRepository'");

            try
            {
                var branch = await _context.Branches.FirstOrDefaultAsync(v => v.Id == id);
                if (branch == null)
                {
                    _logger.LogWarning("Branches with ID {Id} not found.'BranchRepository'", id);
                    return false;
                }

                branch.IsDeleted = true;
                _context.Branches.Update(branch);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully marked Branches with ID {Id} as deleted. 'BranchRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the category as deleted. 'BranchRepository'");
                throw;
            }
        }

    }
}
