namespace UserManagement.API.Dtos.Notification
{
    public class CreateNotificationSystemDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Priority { get; set; }   
    }
}
