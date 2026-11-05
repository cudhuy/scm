namespace UserManagement.API.Dtos.FeedbackCustomer
{
    public class FeedbackCustomerDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

    }
    public class UserFeedbackGroupDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string RankName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public List<FeedbackCustomerDto> Feedbacks { get; set; }

    }
}
