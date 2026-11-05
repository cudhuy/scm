using Branch.API.Dtos.Layout;
using Domain.Entities;

namespace Branch.API.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<List<PromotionalBanner>> GetBannersAsync();
        Task<bool> UpdateBannerAsync(string imageUrl, UpdateBannerFormDto bannerDto);
    }
}
