using UserManagement.API.Dtos;
using UserManagement.API.Dtos.Notification;

namespace UserManagement.API.Services.Interfaces
{
    public interface INotificationsService
    {
        Task<IEnumerable<NotificationSystemDto>> GetAllNotificationSystem();
        Task<IEnumerable<UserWithNotificationsDto>> GetAllNotificationUsers();
        Task<bool> DeleteNotificationSystem(Guid id);
        Task<bool> UpdateNotificationSystem(Guid id, UpdateNotificationSystemDto updateNotificationSystemDto);
        Task<bool> CreateNotificationSystem(CreateNotificationSystemDto createNotificationSystemDto);
        Task<bool> CreateNotificationUser(CreateNotificationUserDto createNotificationUserDto);
        //    Task<IEnumerable<RankWithNotificationsDto>> GetAllNotificationRanks();
     //   Task<IEnumerable<RankWithNotificationsDto>> GetAllNotificationRanks1();
        Task<bool> CreateNotificationForRanks(CreateNotificationRanksDtos createNotificationRanksDtos);
        Task<NotificationResponse> GetAllNotificationSystemForPage(int page, int pageSize, string search);
        // Task<RankNotificationResponse> GetAllNotificationRanksForPage(int page, int pageSize, string search);
        //Task<RankNotificationResponse> GetAllNotificationRanksForPage(int page, int pageSize, string search, Guid? rankId);
        Task<RankNotificationResponse> GetAllNotificationRanksForPage(int page, int pageSize, string search, List<Guid> rankIds);
        Task<NotificationCountsDto> GetNotificationCounts();
        Task<IEnumerable<NotiSystemBaseDto>> GetNotificationSystemBase();


    }
}
