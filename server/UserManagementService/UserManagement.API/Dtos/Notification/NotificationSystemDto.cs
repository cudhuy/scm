using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserManagement.API.Dtos.Notification
{
    public class NotificationSystemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        //  public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public int Priority { get; set; }
        public string PriorityEnum => Enum.IsDefined(typeof(NotificationPriority.NPriority), Priority)
       ? ((NotificationPriority.NPriority)Priority).ToString()
       : "Unknown"; 
      

    }
    public class NotificationResponse
    {
        public List<NotificationSystemDto> Items { get; set; }
        public int? NextPage { get; set; }
    }
}
