namespace UserManagement.API.Dtos.Notification
{
    public class NotificationForRanksDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid RankId { get; set; }
        public string RankName { get; set; }
    }

    public class RankNotificationResponse
    {
        public List<NotificationForRanksDto> Items { get; set; } = new List<NotificationForRanksDto>();
        public int? NextPage { get; set; }
    }
}
