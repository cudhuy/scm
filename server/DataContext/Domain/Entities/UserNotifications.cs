using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserNotifications : BaseEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [ForeignKey("Notification")]
        public Guid NotificationId { get; set; }

        public User User { get; set; }
        public Notification Notification { get; set; }
        public Guid RankId { get; set; }
    }
}
