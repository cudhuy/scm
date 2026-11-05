using Domain.Enums;
using static Domain.Enums.NotificationPriority;

namespace UserManagement.API.Dtos.Notification
{
    public class NotiSystemBaseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Priority { get; set; }

        public string PriorityEnum => Enum.IsDefined(typeof(NotificationPriority.NPriority), Priority)
       ? ((NotificationPriority.NPriority)Priority).ToString()
       : "Unknown";
    }
}
