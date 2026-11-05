using Branch.API.Dtos.Layout;
using Branch.API.Repositories.Interfaces;
using Branch.API.Services.Interfaces;
using Domain.Entities;

namespace Branch.API.Services.Behaviours
{
    public class LayoutService : ILayoutService
    {
        private readonly ILayoutRepository _layoutRepository;
        public LayoutService(ILayoutRepository layoutRepository)
        {
            _layoutRepository = layoutRepository;
        }

        public Task<List<PromotionalBanner>> GetBannersAsync()
        {
            return _layoutRepository.GetBannersAsync();
        }

        public Task<bool> UpdateBannerAsync(string imageUrl, UpdateBannerFormDto banner)
        {
            return _layoutRepository.UpdateBannerAsync(imageUrl, banner);
        }
    }

}
