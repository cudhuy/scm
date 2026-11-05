using Branch.API.Dtos.Layout;
using Domain.Entities;

namespace Branch.API.Repositories.Interfaces
{
    public interface ILayoutRepository
    {
        Task<List<PromotionalBanner>> GetBannersAsync();
        Task<bool> UpdateBannerAsync(string imageUrl, UpdateBannerFormDto banner);
    }
}
