using AutoMapper;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Dtos.Staff;
using UserManagement.API.Helpers;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class StaffInformationService : IStaffInformationService
    {
        private readonly ILogger<StaffInformationService> _logger;
        private readonly IStaffInformationRepository _staffInformationRepository;
        private readonly IMapper _mapper;
        private readonly CloudinaryHelper _cloudinaryHelper;



        public StaffInformationService(ILogger<StaffInformationService> logger, IStaffInformationRepository staffInformationRepository, IMapper mapper, CloudinaryHelper cloudinaryHelper)
        {
            _logger = logger;
            _staffInformationRepository = staffInformationRepository;
            _mapper = mapper;
            _cloudinaryHelper = cloudinaryHelper;


        }

        public async Task<GetStaffsResponseDto> GetAllStaffs(int page, int? pageSize, string? search, Guid? branchId)
        {
            try
            {
                var staffInforList = await _staffInformationRepository.GetListStaffInfor();

                if (staffInforList == null || !staffInforList.Any())
                {
                    _logger.LogInformation("No staff found.");
                    return new GetStaffsResponseDto
                    {
                        Items = Enumerable.Empty<StaffInfoDto>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize ?? 10,
                        TotalPages = 0
                    };
                }

                if (!string.IsNullOrWhiteSpace(search)) staffInforList = staffInforList.Where(st => st.User.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (branchId.HasValue) staffInforList = staffInforList.Where(st => st.Branch.Id == branchId.Value).ToList();

                var totalItems = staffInforList.Count;
                var size = pageSize ?? 10;
                var totalPages = (int)Math.Ceiling(totalItems / (double)size);


                var pagedStaff = staffInforList
                               .Skip((page - 1) * size)
                               .Take(size)
                               .Select(st => new StaffInfoDto
                               {
                                   UserId = st.User.Id,
                                   FullName = st.User.FullName,
                                   Email = st.User.Email,
                                   Phone = st.User.Phone,
                                   Dob = st.User.Dob,
                                   Avatar = st.User.Avatar,
                                   Status = st.User.Status,
                                   RoleName = st.User.Role.RoleName,
                                   Description = st.User.Role.Description,
                                   Title = st.Tittle,
                                   Type = st.Type,
                                   SalaryBase = st.SalaryBase,
                                   BranchName = st.Branch.Name,
                                   BranchAddress = st.Branch.Address,
                                   TypeBranch = st.Branch.Type,
                                   CreatedAt = st.CreatedAt,
                                   UpdatedAt = st.UpdatedAt,
                                   IsDeleted = st.User.IsDeleted
                               })
                               .ToList();

                _logger.LogInformation($"{pagedStaff.Count} staff found in StaffInformationService");

                return new GetStaffsResponseDto
                {
                    Items = pagedStaff,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = size,
                    TotalPages = totalPages
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllStaffs)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> DeleteStaff(Guid id)
        {
            _logger.LogInformation("Entered method DeleteStaff in service for voucher ID {Id}'StaffInformationService'", id);

            try
            {
                var result = await _staffInformationRepository.DeleteStaff(id);
                if (!result)
                {
                    _logger.LogWarning("Staff with ID {Id} not found in service.'StaffInformationService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully marked staff with ID {Id} as deleted in service.'StaffInformationService'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(DeleteStaff)}: {ex.Message}, StackTrace: {ex.StackTrace}'StaffInformationService'");
                throw;
            }
        }

        public async Task<bool> UpdateStaff(UpdateStaffDto updateStaffDto)
        {
            try
            {
                _logger.LogInformation("Entered method UpdateStaff for staff with id {id}.'StaffInformationService'", updateStaffDto.UserId);

                var existingStaff = await _staffInformationRepository.GetStaffById(updateStaffDto.UserId);
                if (existingStaff == null)
                {
                    _logger.LogWarning("Staff with id {id} not found.'StaffInformationService'", updateStaffDto.UserId);
                    return false;
                }


                var result = await _staffInformationRepository.UpdateStaff(updateStaffDto);


                if (!result)
                {
                    _logger.LogWarning("Failed to update staff with id {id}.'StaffInformationService'", updateStaffDto.UserId);
                    return false;
                }

                _logger.LogInformation("Successfully updated staff with id {id}.'StaffInformationService'", updateStaffDto.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(UpdateStaff)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> CreateStaff(CreateStaffDto createStaffDto)
        {
            return await _staffInformationRepository.CreateStaff(createStaffDto);
        }
        public async Task<StaffDetailDto?> GetDetailStaffAsync(Guid userId)
        {
            var user = await _staffInformationRepository.GetUserByIdAsync(userId);
            if (user == null || user.StaffInfo == null)
            {
                return null;
            }

            return new StaffDetailDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                DOB = user.Dob,
                RoleName = user.Role?.RoleName,
                Avatar = user.Avatar,
                Status = user.Status,
                Title = user.StaffInfo.Tittle,
                Type = user.StaffInfo.Type,
                CreateAt = user.CreatedAt,
                SalaryBase = user.StaffInfo.SalaryBase,
                BranchName = user.StaffInfo.Branch?.Name
            };
        }


        public async Task<UserProfileDto> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            try
            {
                _logger.LogInformation("Updating user with ID: {id}", updateProfileDto.UserId);

                var existingUser = await _staffInformationRepository.GetUserById(updateProfileDto.UserId);
                if (existingUser == null)
                {
                    _logger.LogWarning("User ID {id} not found.", updateProfileDto.UserId);
                    return null;
                }

                string imageUrl = existingUser.Avatar ?? string.Empty;
                if (updateProfileDto.Image != null)
                {
                    var uploadResult = await _cloudinaryHelper.UploadImage(new List<IFormFile> { updateProfileDto.Image });

                    if (!uploadResult.IsSuccess || !uploadResult.Files.Any())
                    {
                        throw new InvalidOperationException("Image upload failed. Please try again.");
                    }

                    imageUrl = uploadResult.Files.First().Url;
                }

                await _staffInformationRepository.UpdateProfile(updateProfileDto, imageUrl);
                var user = await _staffInformationRepository.GetUserById(updateProfileDto.UserId);
                return _mapper.Map<UserProfileDto>(user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Cloudinary upload error: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating staff: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<StaffProfileDto?> GetStaffProfileAsync(Guid userId)
        {
            var user = await _staffInformationRepository.GetUserStaffById(userId);
            if (user == null)
            {
                _logger.LogInformation("Staff not found.");
                return null;
            }

            return new StaffProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Dob = user.Dob,
                Address = user.Address,
                IsVerified = user.IsVerified,
                RoleName = user.Role?.RoleName,
                SalaryBase = user.StaffInfo?.SalaryBase ?? 0,
                Tittle = user.StaffInfo?.Tittle,
                Type = user.StaffInfo?.Type,
                BranchName = user.StaffInfo?.Branch?.Name,
                Avatar = user.Avatar,
                Status = user.Status
            };
        }

    }
}
