using AutoMapper;
using Branch.API.Dtos.Branch;
using Branch.API.Dtos.Layout;
using Domain.Entities;

namespace Branch.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateBranchDto, Domain.Entities.Branch>();
            CreateMap<UpdateBranchDto, Domain.Entities.Branch>();
            CreateMap<Domain.Entities.Branch, BranchDetailDto>();
            CreateMap<PromotionalBanner, BannerDto>();
            CreateMap<BannerDto, PromotionalBanner>();
            CreateMap<UpdateBannerFormDto, PromotionalBanner>();

        }
    }
}
