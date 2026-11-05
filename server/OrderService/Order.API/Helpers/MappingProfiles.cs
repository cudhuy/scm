using AutoMapper;
using Domain.Entities;
using Order.API.Dtos.CancelReason;
using Order.API.Dtos.OrderManagement;

namespace Order.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CancelReason, CancelReasonDto>().MaxDepth(64);

            // mapping OrderDto (OrderManagement)
            CreateMap<Domain.Entities.Order, OrderDto>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name)).MaxDepth(64);

            CreateMap<Domain.Entities.Order, OrderOffDto>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
.ForMember(dest => dest.CustomerName,
           opt => opt.MapFrom(src => src.User != null && !string.IsNullOrEmpty(src.User.FullName)
                                     ? src.User.FullName
                                     : "Người dùng chưa đăng ký tài khoản"))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
.ForMember(dest => dest.TableNumber, opt =>
    opt.MapFrom(src =>
        src.Branch.BranchTables
            .Where(bt => bt.Id == src.TableId)
            .Select(bt => bt.TableNumber)
            .FirstOrDefault() ?? null)).MaxDepth(64);

        }
    }
}
