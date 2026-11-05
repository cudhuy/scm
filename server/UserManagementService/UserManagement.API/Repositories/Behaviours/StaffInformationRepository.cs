using AutoMapper;
using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Dtos.Staff;
using UserManagement.API.Helpers;
using UserManagement.API.Messaging.KafkaMessage;
using UserManagement.API.Messaging.Producers;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class StaffInformationRepository : IStaffInformationRepository
    {
        private readonly SCMDbContext _context;
        private readonly ILogger<StaffInformationRepository> _logger;
        private readonly RedisHelper _redisHelper;
        private readonly IMapper _mapper;
        private readonly EmailHelper _emailHelper;
        private readonly CreateStaffProducerService _createStaffProducerService;
        private readonly UpdateProfileProducerService _updateProfileProducerService;

        public StaffInformationRepository(SCMDbContext context, ILogger<StaffInformationRepository> logger
            , RedisHelper redisHelper, IMapper mapper, EmailHelper emailHelper
            , CreateStaffProducerService createStaffProducerService, UpdateProfileProducerService updateProfileProducerService)
        {
            _context = context;
            _logger = logger;
            _redisHelper = redisHelper;
            _mapper = mapper;
            _emailHelper = emailHelper;
            _createStaffProducerService = createStaffProducerService;
            _updateProfileProducerService = updateProfileProducerService;
        }

        public async Task<List<StaffInfo>> GetListStaffInfor()
        {
            _logger.LogInformation("Entered method GetListStaffInfor in 'StaffInformationRepository'");
            try
            {

                var staffList = await _context.StaffInfos
                    .Include(u => u.User)
                        .ThenInclude(r => r.Role)
                    .Where(staff => staff.User != null
                                    && staff.User.Id == staff.UserId
                                    && (staff.User.RoleId == AppConstants.STAFF_ROLE_GUID
                                        || staff.User.RoleId == AppConstants.MANAGER_ROLE_GUID))
                    .OrderBy(x => x.IsDeleted)
                    .Include(b => b.Branch)
                    .ToListAsync();


                _logger.LogInformation("Successfully retrieved {Count} staff.'StaffInformationRepository'", staffList.Count);
                return staffList;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while retrieving staff .'StaffInformationRepository'");
                throw;
            }
        }

        public async Task<bool> DeleteStaff(Guid id)
        {
            _logger.LogInformation("Entered method DeleteStaff in 'StaffInformationRepository'");

            try
            {
                var staff = await _context.Users.FirstOrDefaultAsync(s => s.Id == id && (s.RoleId == AppConstants.STAFF_ROLE_GUID || s.RoleId == AppConstants.MANAGER_ROLE_GUID));
                if (staff == null)
                {
                    _logger.LogWarning("Staff with ID {Id} not found.'StaffInformationRepository'", id);
                    return false;
                }
                if (staff.IsDeleted) staff.IsDeleted = false;
                else if (!staff.IsDeleted) staff.IsDeleted = true;
                _context.Users.Update(staff);

                _logger.LogInformation("Successfully delete staff with ID {Id} as deleted. 'StaffInformationRepository'", id);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the staff  as deleted. 'StaffInformationRepository'");
                throw;
            }
        }

        public async Task<bool> UpdateStaff(UpdateStaffDto updatedStaffInfo)
        {
            _logger.LogInformation("Entered method UpdateStaff in 'StaffRepository'");

            try
            {
                var staffToUpdate = await _context.StaffInfos
                    .Include(s => s.User)
                        .ThenInclude(u => u.Role)
                    .Include(s => s.Branch)
                    .FirstOrDefaultAsync(s => s.UserId == updatedStaffInfo.UserId && !s.User.IsDeleted && (s.User.RoleId == AppConstants.STAFF_ROLE_GUID || s.User.RoleId == AppConstants.MANAGER_ROLE_GUID));
                if (staffToUpdate == null)
                {
                    _logger.LogWarning("Staff with ID {Id} not found to update.", updatedStaffInfo.UserId);
                    return false;
                }
                var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == updatedStaffInfo.BranchId);
                if (branch == null)
                {
                    _logger.LogWarning("Branch with ID {Id} not found.", updatedStaffInfo.BranchId);
                    return false;
                }



                staffToUpdate.Tittle = updatedStaffInfo.Title;
                staffToUpdate.Type = updatedStaffInfo.Type == 1 ? "Hourly" : "Fixed Salary";
                staffToUpdate.SalaryBase = updatedStaffInfo.SalaryBase;
                staffToUpdate.BranchId = updatedStaffInfo.BranchId;
                staffToUpdate.User.Status = updatedStaffInfo.IsActive == true ? "Active" : "Inactive";


                _context.StaffInfos.Update(staffToUpdate);
                _logger.LogInformation("Successfully updated staff with ID {Id}.", updatedStaffInfo.UserId);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the staff.");
                return false;
            }
        }

        public async Task<StaffInfo> GetStaffById(Guid id)
        {
            return await _context.StaffInfos.Include(u => u.User).FirstOrDefaultAsync(v => v.UserId == id && (v.User.RoleId == AppConstants.MANAGER_ROLE_GUID || v.User.RoleId == AppConstants.STAFF_ROLE_GUID));
        }

        public async Task<bool> CreateStaff(CreateStaffDto createStaffDto)
        {
            if (createStaffDto == null) return false;

            var branchExists = await _context.Branches.AnyAsync(b => b.Id == createStaffDto.BranchId);
            if (!branchExists) return false;

            var existingUserId = await _context.Users
                .Where(u => u.Email == createStaffDto.Email && u.RoleId == AppConstants.USER_ROLE_GUID)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            Guid userId = existingUserId;
            if (existingUserId == Guid.Empty)
            {
                User newUser = _mapper.Map<User>(createStaffDto);
                newUser.Id = Guid.NewGuid();
                newUser.RoleId = createStaffDto.IsStaff ? AppConstants.STAFF_ROLE_GUID : AppConstants.MANAGER_ROLE_GUID;
                newUser.Status = UserEnum.EStatus.Active.ToString();
                newUser.RankId = null;

                await _context.Users.AddAsync(newUser);
                userId = newUser.Id;
            }

            await _context.StaffInfos.AddAsync(new StaffInfo
            {
                UserId = userId,
                Tittle = createStaffDto.Title,
                Type = createStaffDto.Type == 1 ? "Lương Theo Giờ" : "Lương Cứng",
                SalaryBase = createStaffDto.SalaryBase,
                BranchId = createStaffDto.BranchId,
                IsDeleted = false
            });

            if (!await Save()) return false;

            // Push to Kafka
            PassWordHelper.CreatePasswordHash(AppConstants.DefaultPassword, out var passwordHash, out var passwordSalt);
            var staffCreatedMessage = new StaffCreatedMessage()
            {
                Id = userId,
                Title = createStaffDto.Title,
                BranchId = createStaffDto.BranchId,
                RoleId = createStaffDto.IsStaff ? AppConstants.STAFF_ROLE_GUID : AppConstants.MANAGER_ROLE_GUID,
                FullName = createStaffDto.FullName,
                Email = createStaffDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Phone = createStaffDto.Phone,
                Status = nameof(UserEnum.EStatus.Active)
            };
            var messageJson = JsonConvert.SerializeObject(staffCreatedMessage);
            await _createStaffProducerService.ProducerMessage(messageJson);

            // Lưu OTP vào Redis và gửi email xác thực
            try
            {
                string otp = new Random().Next(100000, 999999).ToString();
                await _redisHelper.SetRedisAsync(userId.ToString(), otp, TimeSpan.FromMinutes(30));
                bool emailSent = await SendVerificationEmail(createStaffDto.Email, otp);
                if (!emailSent)
                {
                    _logger.LogWarning($"Không thể gửi email OTP cho {createStaffDto.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu OTP vào Redis: {ex.Message}");
            }


            return true;
        }

        public async Task<bool> Save()
        {
            try
            {
                _logger.LogInformation("Entered StaffInformationRepository");
                var saved = await _context.SaveChangesAsync();
                _logger.LogInformation("Leaving StaffInformationRepository");
                return saved > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Lỗi khi lưu vào database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Chi tiết lỗi: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        private async Task<bool> SendVerificationEmail(string email, string otp)
        {
            _logger.LogInformation("Entered method SendVerificationEmail");
            string body = $@"
                <html>
                <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                    <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='padding: 20px; text-align: center;'>
                                <h2 style='color: #333;'>Xin chào {email},</h2>
                                <p style='font-size: 16px; color: #555;'>Bạn đã yêu cầu mã xác minh từ <strong>Xóm</strong>.</p>
                                <p style='font-size: 16px; color: #555;'>Mã xác minh của bạn là:</p>
                                <div style='display: inline-block; padding: 10px 20px; font-size: 24px; font-weight: bold; color: white; background-color: #007bff; border-radius: 5px;'>
                                    {otp}
                                </div>
                                <p style='font-size: 14px; color: #777; margin-top: 10px;'>Mã xác minh của bạn sẽ hết hạn trong <strong>30 phút</strong>.</p>
                                <p style='font-size: 14px; color: #777;'>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>
                                <hr style='border: none; height: 1px; background-color: #ddd; margin: 20px 0;'>
                                <p style='font-size: 14px; color: #999;'>Trân trọng,</p>
                                <p style='font-size: 14px; color: #555; font-weight: bold;'>Đội ngũ Xóm</p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

            _logger.LogInformation("Leaving method SendVerificationEmail");
            return await _emailHelper.SendEmail(email, "Email Verification", body);
        }

        public async Task<StaffInfo> GetByUserId(Guid userId)
        {
            _logger.LogInformation("Entered method GetByUserId StaffInformationRepository");
            var result = await _context.StaffInfos
                .AsNoTracking()
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.UserId == userId);
            _logger.LogInformation("leaving method GetByUserId StaffInformationRepository");
            return result;
        }

        public async Task<List<StaffInfo>> GetByUserIds(List<Guid> userIds)
        {
            return await _context.StaffInfos
                .Where(s => userIds.Contains(s.UserId))
                .Include(s => s.User)
                .ToListAsync();
        }
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.Role)
                .Include(u => u.StaffInfo)
                    .ThenInclude(si => si.Branch)
                .FirstOrDefaultAsync();
        }
        //profile
        public async Task<bool> UpdateProfile(UpdateProfileDto updateProfileDto, string? imageUrl)
        {
            _logger.LogInformation("Updating user with ID {Id}", updateProfileDto.UserId);

            var userToUpdate = await _context.Users
                .FirstOrDefaultAsync(s => s.Id == updateProfileDto.UserId && !s.IsDeleted);

            if (userToUpdate == null)
            {
                _logger.LogWarning("Staff with ID {Id} not found.", updateProfileDto.UserId);
                return false;
            }
            if (!string.IsNullOrWhiteSpace(updateProfileDto.FullName))
            {
                userToUpdate.FullName = updateProfileDto.FullName;
            }
            if (!string.IsNullOrWhiteSpace(updateProfileDto.Phone))
            {
                userToUpdate.Phone = updateProfileDto.Phone;
            }
            if (updateProfileDto.Dob.HasValue)
            {
                userToUpdate.Dob = updateProfileDto.Dob.Value;
            }
            if (!string.IsNullOrWhiteSpace(updateProfileDto.Address))
            {
                userToUpdate.Address = updateProfileDto.Address;
            }
            if (imageUrl != null)
            {
                userToUpdate.Avatar = imageUrl;
            }
            userToUpdate.UpdatedBy = updateProfileDto.UserId;
            userToUpdate.UpdatedAt = DateTime.Now;
            _context.Users.Update(userToUpdate);
            var messageJson = JsonConvert.SerializeObject(userToUpdate);
            await _updateProfileProducerService.ProducerMessage(messageJson);
            return await Save();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        }

        public async Task<User> GetUserStaffById(Guid id)
        {
            _logger.LogInformation("Entered method GetUserStaffById in 'StaffInformationRepository'");

            var staff = await _context.Users
                .Where(u => u.Id == id && u.RoleId != AppConstants.USER_ROLE_GUID && !u.IsDeleted)
                .Include(u => u.StaffInfo).ThenInclude(s => s.Branch)
                .Include(u => u.Role)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Successfully retrieved staff with ID {Id} in 'StaffInformationRepository'", id);

            return staff;
        }


    }
}
