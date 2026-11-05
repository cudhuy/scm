namespace Branch.API.Dtos.Feedback
{
    public class FeedbackDto
    {
        public string CustomerName { get; set; }
        public string OrderCode { get; set; }
        public string BranchName { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class FeedbackResponse
    {
        public List<FeedbackDto> Items { get; set; } = new List<FeedbackDto>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
