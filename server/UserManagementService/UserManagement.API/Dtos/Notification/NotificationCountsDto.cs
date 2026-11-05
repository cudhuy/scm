namespace UserManagement.API.Dtos.Notification
{
    public class NotificationCountsDto
    {
        public int UserNotificationsCount { get; set; }
        public int SystemNotificationsCount { get; set; }
        public int TotalNotificationsCount { get; set; }
        public int NotificationsTodayCount { get; set; }
    }
}
