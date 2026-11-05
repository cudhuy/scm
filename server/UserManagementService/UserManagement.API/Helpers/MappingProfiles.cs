using AutoMapper;
using Domain.Entities;
using UserManagement.API.Dtos;
using UserManagement.API.Dtos.Customer;
using UserManagement.API.Dtos.FeedbackCustomer;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Dtos.Staff;
using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<MembershipRanks, MemberShipRankDto>().MaxDepth(64);
            CreateMap<CreateStaffDto, User>().MaxDepth(64);
            CreateMap<User, CustomerDto>().MaxDepth(64);
            CreateMap<User, UserProfileDto>().MaxDepth(64);
            CreateMap<Voucher, CreateNewVoucherDto>()
               .ForMember(dest => dest.MembershipRankIds, opt => opt.MapFrom(src => src.MembershipRanksVouchers.Select(m => m.MembershipRankId).ToList())).MaxDepth(64);

            CreateMap<CreateNewVoucherDto, Voucher>()
              .ForMember(dest => dest.MembershipRanksVouchers, opt => opt.MapFrom(src => src.MembershipRankIds.Select(id => new MembershipRanksVouchers { MembershipRankId = id }).ToList())).MaxDepth(64);
            CreateMap<Voucher, UpdateVoucherDto>()
              .ForMember(dest => dest.MembershipRankIds, opt => opt.MapFrom(src => src.MembershipRanksVouchers.Select(m => m.MembershipRankId).ToList())).MaxDepth(64);
            CreateMap<UpdateVoucherDto, Voucher>()
              .ForMember(dest => dest.MembershipRanksVouchers, opt => opt.MapFrom(src => src.MembershipRankIds.Select(id => new MembershipRanksVouchers { MembershipRankId = id }).ToList())).MaxDepth(64);
            // Mapping Staff

            CreateMap<UpdateUserDto, User>().MaxDepth(64);

            // Mapping role
            CreateMap<CreateRoleDto, UserRole>().MaxDepth(64);
            CreateMap<UpdateRoleDto, UserRole>().MaxDepth(64);


            // Mapping Notification
            CreateMap<NotificationSystemDto, Notification>().MaxDepth(64);
            CreateMap<NotificationUserDto, Notification>().MaxDepth(64);
            CreateMap<Notification, NotificationUserDto>().MaxDepth(64);
            CreateMap<UserNotifications, UserWithNotificationsDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.User.Ranks.Name))
            .ForMember(dest => dest.Notification, opt => opt.MapFrom(src => src.Notification)).MaxDepth(64);
            CreateMap<UpdateNotificationSystemDto, Notification>().MaxDepth(64);
            CreateMap<CreateNotificationSystemDto, Notification>().MaxDepth(64);
            CreateMap<CreateNotificationUserDto, UserNotifications>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Notification, opt => opt.MapFrom(src => new Notification
            {
                Title = src.Title,
                Content = src.Content
            })).MaxDepth(64);

            CreateMap<Notification, NotificationForRanksDto>().MaxDepth(64);

            // Mapping WorkShift
            CreateMap<UpdateWorkShiftDto, WorkShift>().MaxDepth(64);
            CreateMap<WorkShift, WorkShiftResponseDto>().MaxDepth(64);

            // Mapping EmployeeShift
            CreateMap<CreateEmployeeShiftDto, EmployeeShift>().MaxDepth(64);
            CreateMap<User, StaffInShift>().MaxDepth(64);
            CreateMap<EmployeeShift, ScheduleDetail>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.EmployeeShiftId, opt => opt.MapFrom(src => src.Id)).MaxDepth(64);

            CreateMap<List<EmployeeShift>, StaffScheduleResponse>()
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.First().BranchId))
            .ForMember(dest => dest.ScheduleDetail, opt => opt.MapFrom(src => src)).MaxDepth(64);

            //Mapping feedback
            CreateMap<Feedback, UserFeedbackGroupDto>()
      .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
      .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
      .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.User.Ranks.Name))
      .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
      .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status)).MaxDepth(64);
            CreateMap<Feedback, FeedbackCustomerDto>().MaxDepth(64);

            // Mapping Salary
            CreateMap<Salary, MonthlySalaryResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName)).MaxDepth(64);

            CreateMap<CreateMonthlySalaryDto, Salary>()
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => src.TotalHour)).MaxDepth(64);
        }
    }
}
