namespace Branch.API.Dtos.Layout
{
    public class UpdateBannerFormDto
    {
        public Guid Id { get; set; }
        public string? Title1 { get; set; }
        public string? Title2 { get; set; }

        public string? Subtitle { get; set; }

        public string? DiscountLabel { get; set; }

        public IFormFile? ImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
