using Domain.Entities.Base;

namespace Domain.Entities
{
    public class PromotionalBanner : BaseEntity
    {
        public string? Title1 { get; set; } // Ví dụ: "STAY HOME & DELIVERD"
        public string? Title2 { get; set; } // Ví dụ: "YOUR DAILY NEEDS"

        public string? Subtitle { get; set; } // Ví dụ: "Only this week. Don’t miss."

        public string? DiscountLabel { get; set; } // Ví dụ: "30% OFF", "Giảm 45%"

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
