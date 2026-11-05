using Domain.Entities;

namespace UserManagement.API.Repositories.Interfaces
{
    public interface INotificationsRepository
    {
        Task<List<Notification>> GetListNotificationSystem();
        Task<List<UserNotifications>> GetListNotificationUser();
        Task<bool> DeletetNotificationSystem(Guid id);
        Task<bool> UpdateNotificationSystem(Guid id, Notification updatedNotification);
        Task<Notification> GetNotificationSystemById(Guid id);
        Task<bool> CreateNotificationSystem(Notification createNotification);
        Task<bool> CreateNotificationUser(UserNotifications createNotification);
        Task<String> GetUserEmailByIdAsync(Guid id);
     //   Task<List<UserNotifications>> GetListNotificationRank();
      //  Task<List<UserNotifications>> GetListNotificationRank1();

        Task<List<User>> GetUsersByRank(List<Guid> rankIds);
        Task<bool> CreateNotificationForRank(Notification notification);
        Task<bool> CreateBulkUserNotifications(List<UserNotifications> userNotifications);
        Task<List<MembershipRanks>> GetRanksByIds(List<Guid> rankIds);
         Task<Dictionary<Guid, string>> GetRankNames(IEnumerable<Guid> rankIds);
        Task<(List<Notification>, bool)> GetListNotificationSystemForPage(int page, int pageSize, string search);
        //   Task<List<UserNotifications>> GetListNotificationRankForPage(int page, int pageSize, string search);
        //   Task<List<UserNotifications>> GetListNotificationRankForPage(int page, int pageSize, string search, Guid? rankId);
        Task<List<UserNotifications>> GetListNotificationRankForPage(int page, int pageSize, string search, List<Guid> rankIds);
        Task<(int userNotificationsCount, int systemNotificationsCount)> GetNotificationCounts();
        Task<int> GetNotificationsCountToday();
        Task<List<Notification>> GetListNotificationSystemBase();


    }
}
