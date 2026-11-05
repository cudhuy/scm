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
            CreateMap<MembershipRanks, MemberShipRankDto>();
            CreateMap<CreateStaffDto, User>();
            CreateMap<User, CustomerDto>();
            CreateMap<User, UserProfileDto>();
            CreateMap<Voucher, CreateNewVoucherDto>()
               .ForMember(dest => dest.MembershipRankIds, opt => opt.MapFrom(src => src.MembershipRanksVouchers.Select(m => m.MembershipRankId).ToList()));

            CreateMap<CreateNewVoucherDto, Voucher>()
              .ForMember(dest => dest.MembershipRanksVouchers, opt => opt.MapFrom(src => src.MembershipRankIds.Select(id => new MembershipRanksVouchers { MembershipRankId = id }).ToList()));
            CreateMap<Voucher, UpdateVoucherDto>()
              .ForMember(dest => dest.MembershipRankIds, opt => opt.MapFrom(src => src.MembershipRanksVouchers.Select(m => m.MembershipRankId).ToList()));
            CreateMap<UpdateVoucherDto, Voucher>()
              .ForMember(dest => dest.MembershipRanksVouchers, opt => opt.MapFrom(src => src.MembershipRankIds.Select(id => new MembershipRanksVouchers { MembershipRankId = id }).ToList()));
            // Mapping Staff

            CreateMap<UpdateUserDto, User>();

            // Mapping role
            CreateMap<CreateRoleDto, UserRole>();
            CreateMap<UpdateRoleDto, UserRole>();


            // Mapping Notification
            CreateMap<NotificationSystemDto, Notification>();
            CreateMap<NotificationUserDto, Notification>();
            CreateMap<Notification, NotificationUserDto>();
            CreateMap<UserNotifications, UserWithNotificationsDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.User.Ranks.Name))
            .ForMember(dest => dest.Notification, opt => opt.MapFrom(src => src.Notification));
            CreateMap<UpdateNotificationSystemDto, Notification>();
            CreateMap<CreateNotificationSystemDto, Notification>();
            CreateMap<CreateNotificationUserDto, UserNotifications>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Notification, opt => opt.MapFrom(src => new Notification
            {
                Title = src.Title,
                Content = src.Content
            }));

            CreateMap<Notification, NotificationForRanksDto>();

            // Mapping WorkShift
            CreateMap<UpdateWorkShiftDto, WorkShift>();
            CreateMap<WorkShift, WorkShiftResponseDto>();

            // Mapping EmployeeShift
            CreateMap<CreateEmployeeShiftDto, EmployeeShift>();
            CreateMap<User, StaffInShift>();
            CreateMap<EmployeeShift, ScheduleDetail>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.EmployeeShiftId, opt => opt.MapFrom(src => src.Id));

            CreateMap<List<EmployeeShift>, StaffScheduleResponse>()
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.First().BranchId))
            .ForMember(dest => dest.ScheduleDetail, opt => opt.MapFrom(src => src));

            //Mapping feedback
            CreateMap<Feedback, UserFeedbackGroupDto>()
      .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
      .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
      .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.User.Ranks.Name))
      .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
      .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status));
            CreateMap<Feedback, FeedbackCustomerDto>();

            // Mapping Salary
            CreateMap<Salary, MonthlySalaryResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));

            CreateMap<CreateMonthlySalaryDto, Salary>()
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => src.TotalHour));
        }
    }
}
