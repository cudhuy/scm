using Branch.API.Dtos.Layout;
using Branch.API.Repositories.Interfaces;
using Branch.API.Services.Behaviours;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Branch.API.Repositories.Behaviours
{
    public class LayoutRepository : ILayoutRepository
    {
        private readonly SCMDbContext _context;
        private readonly ILogger<ReportRepository> _logger;

        public LayoutRepository(SCMDbContext context, ILogger<ReportRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PromotionalBanner>> GetBannersAsync()
        {
            return await _context.PromotionalBanners
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateBannerAsync(string imageUrl, UpdateBannerFormDto banner)
        {
            var existingBanner = await _context.PromotionalBanners
                .FirstOrDefaultAsync(x => x.Id == banner.Id);

            if (existingBanner != null)
            {
                if (!string.IsNullOrEmpty(banner.Title1)) existingBanner.Title1 = banner.Title1;

                if (!string.IsNullOrEmpty(banner.Title2)) existingBanner.Title2 = banner.Title2;

                if (!string.IsNullOrEmpty(banner.Subtitle)) existingBanner.Subtitle = banner.Subtitle;

                if (!string.IsNullOrEmpty(banner.DiscountLabel)) existingBanner.DiscountLabel = banner.DiscountLabel;

                if (imageUrl != null) existingBanner.ImageUrl = imageUrl;

                _context.PromotionalBanners.Update(existingBanner);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                _logger.LogWarning($"Banner with ID {banner.Id} not found.");
                return false;
            }
        }
    }

}
