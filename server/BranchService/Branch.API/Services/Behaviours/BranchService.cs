using AutoMapper;
using Branch.API.Dtos.Branch;
using Branch.API.Services.Interfaces;

namespace Branch.API.Services.Behaviours
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchService> _logger;

        public BranchService(IBranchRepository branchRepository, IMapper mapper, ILogger<BranchService> logger)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BranchDto>> GetAllBranch()
        {
            _logger.LogInformation("Enter method GetAllBranch. 'BranchService'");

            try
            {
                var branchs = await _branchRepository.GetListBranch();

                if (branchs == null || !branchs.Any())
                {
                    _logger.LogInformation("No branch found.");
                    return Enumerable.Empty<BranchDto>();
                }

                var branchDtos = new List<BranchDto>();

                foreach (var c in branchs)
                {
                    var branchDto = new BranchDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Address = c.Address,
                        Email = c.Email,
                        Phone = c.Phone,
                        Type = c.Type



                    };
                    branchDtos.Add(branchDto);
                }

                _logger.LogInformation($"{branchDtos.Count} BranchBaseDto found.");
                return branchDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllBranch)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> CreateBranch(CreateBranchDto createBranchDto)
        {
            try
            {
                _logger.LogInformation("Creating a new Branch. 'BranchService'");

                var branch = _mapper.Map<Domain.Entities.Branch>(createBranchDto);
                var result = await _branchRepository.CreateBranch(branch);

                if (!result)
                {
                    _logger.LogWarning("Failed to create category. 'BranchService'");
                    return false;
                }

                _logger.LogInformation("Category created successfully. 'BranchService'");
                return true;
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "Business logic error in CreateBranch.");
                throw;
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid branch data.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating category. 'BranchService'");
                throw;
            }


        }

        public async Task<bool> UpdateBranch(Guid id, UpdateBranchDto updateBranchDto)
        {
            if (updateBranchDto == null)
            {
                throw new ArgumentNullException(nameof(updateBranchDto), "UpdateBranchDto cannot be null.");
            }

            _logger.LogInformation("Entered UpdateBranch in service for branch with id {id}.", id);

            try
            {
                var existingBranch = await _branchRepository.GetBranchById(id);
                if (existingBranch == null)
                {
                    _logger.LogWarning("Branch with id {id} not found.", id);
                    throw new KeyNotFoundException($"Branch with id {id} not found.");
                }

                var updatedBranch = _mapper.Map<Domain.Entities.Branch>(updateBranchDto);
                if (updatedBranch == null)
                {
                    _logger.LogWarning("Mapping UpdateBranchDto to Branch entity failed.");
                    throw new InvalidOperationException("Failed to map UpdateBranchDto to Branch entity.");
                }

                var resultBranchUpdate = await _branchRepository.UpdateBranch(id, updatedBranch);
                if (resultBranchUpdate == null)
                {
                    _logger.LogWarning("Failed to update Branch with id {id}.", id);
                    throw new Exception($"Failed to update Branch with id {id}.");
                }

                _logger.LogInformation("Successfully updated Branch with id {id}.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(UpdateBranch)}: {ex.Message}, StackTrace: {ex.StackTrace}.");
                throw;
            }
        }



        public async Task<bool> DeleteBranch(Guid id)
        {
            _logger.LogInformation("Entered method DeleteBranch in service for category ID {Id}'BranchService'", id);

            try
            {
                var existingBranch = await _branchRepository.GetBranchById(id);
                if (existingBranch == null)
                {
                    _logger.LogWarning("Branch with ID {Id} not found.", id);
                    return false;
                };
                if (existingBranch.Type == "Main")
                {
                    _logger.LogWarning("Attempted to delete a Main branch with ID {Id}, which is not allowed.", id);
                    throw new InvalidOperationException("Cannot delete a Main branch.");
                }


                var result = await _branchRepository.DeleteBranch(id);
                if (!result)
                {
                    _logger.LogWarning("Branch with ID {Id} not found in service.'BranchService'", id);
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
        public async Task<BranchDetailDto> GetBranchById(Guid id)
        {
            _logger.LogInformation("Fetching details for Branch ID {Id}.", id);

            try
            {
                var branch = await _branchRepository.GetBranchById(id);
                if (branch == null)
                {
                    _logger.LogWarning("Branch with ID {Id} not found.", id);
                    return null;
                }

                var branchDto = _mapper.Map<BranchDetailDto>(branch);
                return branchDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetBranchById)}: {ex.Message}, StackTrace: {ex.StackTrace}.");
                throw;
            }
        }

        public async Task<IEnumerable<BranchBaseDto>> GetAllBranchBase()
        {
            _logger.LogInformation("Enter method GetAllBranch. 'BranchService'");

            try
            {
                var branchs = await _branchRepository.GetListBranch();

                if (branchs == null || !branchs.Any())
                {
                    _logger.LogInformation("No branch found.");
                    return Enumerable.Empty<BranchBaseDto>();
                }

                var branchDtos = new List<BranchBaseDto>();

                foreach (var c in branchs)
                {
                    var branchDto = new BranchBaseDto
                    {
                        Id = c.Id,
                        Name = c.Name,




                    };
                    branchDtos.Add(branchDto);
                }

                _logger.LogInformation($"{branchDtos.Count} BranchBaseDto found.");
                return branchDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllBranch)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }



    }

}
