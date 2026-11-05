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
            CreateMap<CreateBranchDto, Domain.Entities.Branch>().MaxDepth(64);
            CreateMap<UpdateBranchDto, Domain.Entities.Branch>().MaxDepth(64);
            CreateMap<Domain.Entities.Branch, BranchDetailDto>().MaxDepth(64);
            CreateMap<PromotionalBanner, BannerDto>().MaxDepth(64);
            CreateMap<BannerDto, PromotionalBanner>().MaxDepth(64);
            CreateMap<UpdateBannerFormDto, PromotionalBanner>().MaxDepth(64);

        }
    }
}
