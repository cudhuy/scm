using AutoMapper;
using UserManagement.API.Dtos;
using UserManagement.API.Dtos.MembershipRank;
using Domain.Entities;
using Domain.Enums;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;
using UserManagement.API.Dtos.Notification;

namespace UserManagement.API.Services.Behaviours
{
    public class VoucherService : IVoucherService
    {
        private readonly ILogger<VoucherService> _logger;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMembershipRanksRepository _membershipRanksRepository;
        private readonly IMapper _mapper;

        public VoucherService(ILogger<VoucherService> logger, IVoucherRepository voucherRepository, IMapper mapper, IMembershipRanksRepository membershipRanksRepository)
        {
            _logger = logger;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
            _membershipRanksRepository = membershipRanksRepository;
        }

        public async Task<IEnumerable<VoucherDto>> GetAllVouchers()
        {
            try
            {
                var vouchers = await _voucherRepository.GetListVoucher();

                if (vouchers == null || !vouchers.Any())
                {
                    _logger.LogInformation("No vouchers found.");
                    return Enumerable.Empty<VoucherDto>();
                }

                var voucherDtos = new List<VoucherDto>();

                foreach (var v in vouchers)
                {
                    var voucherDto = new VoucherDto
                    {
                        VoucherId = v.Id,
                        Code = v.Code,
                        NoOfVoucher = v.NoOfVoucher,
                        Description = v.Description,
                        DiscountValue = v.DiscountValue,
                        DiscountType = (VoucherEnum.EDiscountType)v.DiscountType,
                        MinOrder = v.MinOrder,
                        StartDate = v.StartDate,
                        ExpireDate = v.ExpireDate,
                        UsageCount = v.noOfVoucherUsed,
                        CreatedAt = v.CreatedAt,
                        CreatedBy = v.CreatedBy,
                        UpdatedAt = v.UpdatedAt,
                        UpdatedBy = v.UpdatedBy,
                        IsDeleted = v.IsDeleted,

                        MembershipRanks = v.MembershipRanksVouchers.Select(mrv => new MembershipRankOfVoucherDto
                        {
                            Id = mrv.MembershipRank.Id,
                            Name = mrv.MembershipRank.Name,
                            Description = mrv.MembershipRank.Description
                        }).ToList()
                    };

                    // Đếm số lần sử dụng voucher này
                  //  var usageCount = await _voucherRepository.GetVoucherUsageCount(v.Id);
                  //  voucherDto.UsageCount = usageCount;

                    voucherDtos.Add(voucherDto);
                }

                _logger.LogInformation($"{voucherDtos.Count} vouchers found.");
                return voucherDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllVouchers)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }


        public async Task<IEnumerable<VoucherUsagesDto>> GetAllVoucherUseds()
        {
            try
            {
                var voucherUseds = await _voucherRepository.GetListVoucherUsed();

                if (voucherUseds == null || !voucherUseds.Any())
                {
                    _logger.LogInformation("No voucher Useds found.");
                    return Enumerable.Empty<VoucherUsagesDto>();
                }

                var voucherUsedDtos = voucherUseds.Select(vu => new VoucherUsagesDto
                {
                    VoucherId = vu.Voucher.Id,
                    VoucherDescription = vu.Voucher.Description,
                    DiscountValue = vu.Voucher.DiscountValue,
                    DiscountType = vu.Voucher.DiscountType,
                    StartDate = vu.Voucher.StartDate,
                    ExpireDate = vu.Voucher.ExpireDate,
                    UsedAt = vu.UsedAt,
                    UserId = vu.User.Id,
                    UserFullName = vu.User.FullName,
                    UserEmail = vu.User.Email,
                }).ToList();

                _logger.LogInformation($"{voucherUsedDtos.Count} vouchers usages found.'VoucherService'");
                return voucherUsedDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllVoucherUseds)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        public async Task<bool> CreateVoucher(CreateNewVoucherDto createVoucherDto)
        {
            _logger.LogInformation("Entered method CreateVoucher in service  to create a voucher with code {VoucherCode}'VoucherService'", createVoucherDto.Code);

            var existingVoucher = await _voucherRepository.GetVoucherByCode(createVoucherDto.Code);
            if (existingVoucher != null)
            {
                _logger.LogWarning("Voucher with code {VoucherCode} already exists.'VoucherService'", createVoucherDto.Code);
                return false;
            }

            var membershipRanks = await _membershipRanksRepository.GetMembershipRanksByIds(createVoucherDto.MembershipRankIds);
            if (membershipRanks == null || membershipRanks.Count == 0)
            {
                _logger.LogWarning("Invalid Membership Rank IDs provided for voucher creation.'VoucherService'");
                return false;
            }

            var voucher = _mapper.Map<Voucher>(createVoucherDto);
            var createdVoucher = await _voucherRepository.CreateVoucher(voucher, createVoucherDto.MembershipRankIds);
            _logger.LogInformation("Successfully created voucher with code {VoucherCode}.'VoucherService'");

            return true;
        }

        public async Task<bool> DeleteVoucher(Guid id)
        {
            _logger.LogInformation("Entered method DeleteVoucher in service for voucher ID {Id}'VoucherService'", id);

            try
            {
                var result = await _voucherRepository.DeleteVoucher(id);
                if (!result)
                {
                    _logger.LogWarning("Voucher with ID {Id} not found in service.'VoucherService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully marked voucher with ID {Id} as deleted in service.'VoucherService'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(DeleteVoucher)}: {ex.Message}, StackTrace: {ex.StackTrace}'VoucherService'");
                throw;
            }
        }

        public async Task<bool> UpdateVoucher(Guid id, UpdateVoucherDto updateVoucherDto)
        {
            _logger.LogInformation("Entered method UpdateVoucher in service for voucher with id {id}.'VoucherService'", id);

            try
            {
                var existingVoucher = await _voucherRepository.GetVoucherById(id);
                if (existingVoucher == null)
                {
                    _logger.LogWarning("Voucher with id {id} not found.'VoucherService'", id);
                    return false;
                }

                // Kiểm tra Membership Rank IDs hợp lệ
                var membershipRanks = await _membershipRanksRepository.GetMembershipRanksByIds(updateVoucherDto.MembershipRankIds);
                if (membershipRanks == null || membershipRanks.Count == 0)
                {
                    _logger.LogWarning("Invalid Membership Rank IDs provided for voucher update.'VoucherService'");
                    return false;
                }

                var updatedVoucher = _mapper.Map<Voucher>(updateVoucherDto);
                updatedVoucher.Id = existingVoucher.Id;

                var resultVoucher = await _voucherRepository.UpdateVoucher(existingVoucher.Id, updatedVoucher, updateVoucherDto.MembershipRankIds);

                if (resultVoucher == null)
                {
                    _logger.LogWarning("Failed to update voucher with code {Code}.'VoucherService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully updated voucher with code {Code}.'VoucherService'", id);

                // Trả về DTO đã cập nhật
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(UpdateVoucher)}: {ex.Message}, StackTrace: {ex.StackTrace}.'VoucherService'");
                throw;
            }
        }

        //user 
        public async Task<IEnumerable<VoucherUserDto>> GetUserVouchers(Guid userId)
        {
            try
            {
                var user = await _voucherRepository.GetUserById(userId);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {userId} not found.");
                }
                var membershipRank = user.Ranks;
                if (membershipRank == null) return Enumerable.Empty<VoucherUserDto>();

                var membershipRankVouchers = await _voucherRepository.GetMembershipRankVouchers(membershipRank.Id);

                var voucherDtos = new List<VoucherUserDto>();

                foreach (var membershipRankVoucher in membershipRankVouchers)
                {
                    var voucher = await _voucherRepository.GetVoucherById(membershipRankVoucher.VoucherId);
                    if (voucher != null)
                    {
                        var voucherDto = new VoucherUserDto
                        {
                            Id = voucher.Id,
                            Code = voucher.Code,
                            Description = voucher.Description,
                            DiscountValue = voucher.DiscountValue,
                            ExpireDate = voucher.ExpireDate,
                            MinOrder = voucher.MinOrder,
                            DiscountType = voucher.DiscountType
                        };

                        voucherDtos.Add(voucherDto);
                    }
                }

                return voucherDtos;
            }
            catch (KeyNotFoundException ex)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in {nameof(GetUserVouchers)}: {ex.Message}", ex);
            }
        }




    }
}
