namespace UserManagement.API.Dtos.Notification
{
    public class NotificationUserDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Type { get; set; } // 1: System, 2: User
        public DateTime CreatedAt { get; set; }


    }
    public class UserWithNotificationsDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RankName { get; set; }
        public NotificationUserDto Notification { get; set; }
    }
}
