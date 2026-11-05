namespace UserManagement.API.Dtos.Notification
{
    public class CreateNotificationRanksDtos
    {
        public List<Guid> RankIds { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
