using Domain.Entities.Base;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Type { get; set; } // 1: System, 2: User
        public int Priority { get; set; } // 1: Low, 2: Medium, 3: High
        public ICollection<UserNotifications> UserNotifications { get; set; }


    }
}
