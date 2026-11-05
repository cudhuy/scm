namespace UserManagement.API.Dtos.Notification
{
    public class CreateNotificationUserDto
    {
        public Guid UserId { get; set; }   
        public string Title { get; set; }   
        public string Content { get; set; } 
    }
}
