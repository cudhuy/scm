using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UserManagement.API.Repositories.Interfaces;

namespace UserManagement.API.Repositories.Behaviours
{
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly SCMDbContext _context;

        private readonly ILogger<NotificationsRepository> _logger;

        public NotificationsRepository(SCMDbContext context, ILogger<NotificationsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Notification>> GetListNotificationSystem()
        {
            _logger.LogInformation("Entered method GetListNotification in NotificationsRepository");

            try
            {
                var notifications = await _context.Notifications
                    .Where(n => !n.IsDeleted && n.Type == (int)NotificationEnum.EType.System)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();


                _logger.LogInformation("Successfully retrieved {Count} notification system .", notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                throw;
            }
        }

        public async Task<List<UserNotifications>> GetListNotificationUser()
        {
            _logger.LogInformation("Entered method GetListNotificationUser in NotificationsRepository");

            try
            {
                var notifications = await _context.UserNotifications
                    .Include(u => u.User).ThenInclude(r => r.Ranks)
                    .Include(n => n.Notification)
                    .Where(n => !n.IsDeleted && n.Notification.Type == (int)NotificationEnum.EType.User && n.User.StaffInfo == null)
                    .ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} notification user.", notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                throw;
            }
        }
        public async Task<bool> DeletetNotificationSystem(Guid id)
        {
            _logger.LogInformation("Entered method DeletetNotificationSystem in 'NotificationsRepository'");

            try
            {
                var notificationSystem = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
                if (notificationSystem == null)
                {
                    _logger.LogWarning("Notification with ID {Id} not found.'NotificationsRepository'", id);
                    return false;
                }

                notificationSystem.IsDeleted = true;
                _context.Notifications.Update(notificationSystem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully marked notification with ID {Id} as deleted. 'NotificationsRepository'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the notification as deleted. 'NotificationsRepository'");
                throw;
            }
        }

        public async Task<bool> UpdateNotificationSystem(Guid id, Notification updatedNotification)
        {
            _logger.LogInformation("Entered method UpdateNotificationSystem in 'NotificationsRepository'");

            try
            {
                var notification = await _context.Notifications.FirstOrDefaultAsync(v => v.Id == id);
                if (notification == null)
                {
                    _logger.LogWarning("notification with ID {Id} not found to update. 'NotificationsRepository'", id);
                    return false;
                }

                notification.Content = updatedNotification.Content;
                notification.UpdatedAt = DateTime.Now;

                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated notification with ID {Id}.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the notification.");
                return false;
            }
        }

        public async Task<bool> CreateNotificationSystem(Notification createNotification)
        {
            _logger.LogInformation("Entered method CreateNotificationSystem in NotificationsRepository");

            try
            {
                createNotification.CreatedAt = DateTime.Now;
                createNotification.UpdatedAt = DateTime.Now;
                createNotification.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                createNotification.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;
                createNotification.Type = (int)NotificationEnum.EType.System;
                createNotification.IsDeleted = false;

                await _context.Notifications.AddAsync(createNotification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added new notification system  with ID {Id}.", createNotification.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the user role.");
                return false;
            }
        }

        public async Task<Notification> GetNotificationSystemById(Guid id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(v => v.Id == id);
        }


        public async Task<bool> CreateNotificationUser(UserNotifications userNotification)
        {
            _logger.LogInformation("Entered method CreateNotificationUser in NotificationsRepository");

            try
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = userNotification.Notification.Title,
                    Content = userNotification.Notification.Content,
                    Type = (int)NotificationEnum.EType.User,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    UpdatedBy = AppConstants.SYSTEM_ROLE_GUID,
                    IsDeleted = false
                };

                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully created Notification with ID {NotificationId}.", notification.Id);
                userNotification.NotificationId = notification.Id; // Liên kết Notification với UserNotification
                userNotification.Notification = null;
                userNotification.UserId = userNotification.UserId;
                userNotification.CreatedAt = DateTime.Now;
                userNotification.UpdatedAt = DateTime.Now;
                userNotification.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                userNotification.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;
                userNotification.IsDeleted = false;

                await _context.UserNotifications.AddAsync(userNotification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added new UserNotification with ID {UserNotificationId}.", userNotification.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the user notification.");
                return false;
            }
        }

        public async Task<string> GetUserEmailByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            return user?.Email;
        }

       
        public async Task<List<UserNotifications>> GetListNotificationRankForPage(int page, int pageSize, string search, List<Guid> rankIds)
        {
            _logger.LogInformation("Entered method GetListNotificationRankForPage in NotificationsRepository");

            try
            {
                var query = _context.UserNotifications
                    .Include(n => n.Notification)
                    .Where(n => !n.IsDeleted && n.RankId != null && n.Notification.Type == (int)NotificationEnum.EType.User)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(n => n.Notification.Title.Contains(search));
                }

                if (rankIds != null && rankIds.Any())
                {
                    query = query.Where(n => rankIds.Contains(n.RankId));
                }

                var notifications = await query
                    .OrderByDescending(n => n.Notification.CreatedAt)
                    .GroupBy(n => new { n.NotificationId, n.RankId })
                    .Select(g => g.First())
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} notifications.", notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                throw;
            }
        }







        public async Task<Dictionary<Guid, string>> GetRankNames(IEnumerable<Guid> rankIds)
        {
            _logger.LogInformation("Entered method GetRankNames in NotificationRepository");

            try
            {
                var rankNames = await _context.MembershipRanks
                    .Where(r => rankIds.Contains(r.Id))
                    .ToDictionaryAsync(r => r.Id, r => r.Name);

                return rankNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving rank names.");
                throw;
            }
        }



        public async Task<List<User>> GetUsersByRank(Guid rankID)
        {
            return await _context.Users
                .Where(u => u.Ranks.Id == rankID)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByRank(List<Guid> rankIds)
        {
            try
            {
                var users = await _context.Users
                                           .Where(u => rankIds.Contains((Guid)u.RankId))
                                           .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUsersByRank: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CreateNotificationForRank(Notification notification)
        {
            try
            {
                notification.CreatedAt = DateTime.Now;
                notification.UpdatedAt = DateTime.Now;
                notification.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                notification.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;
                notification.Type = (int)NotificationEnum.EType.User;
                notification.IsDeleted = false;
                _context.Notifications.Add(notification);
                int result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateNotificationForRank: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> CreateBulkUserNotifications(List<UserNotifications> userNotifications)
        {
            try
            {
                foreach (var userNotification in userNotifications)
                {
                    userNotification.CreatedAt = DateTime.Now;
                    userNotification.UpdatedAt = DateTime.Now;
                    userNotification.CreatedBy = AppConstants.SYSTEM_ROLE_GUID;
                    userNotification.UpdatedBy = AppConstants.SYSTEM_ROLE_GUID;
                    userNotification.IsDeleted = false;
                }
                _context.UserNotifications.AddRange(userNotifications);
                int result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateBulkUserNotifications: {ex.Message}");
                return false;
            }
        }
        public async Task<List<MembershipRanks>> GetRanksByIds(List<Guid> rankIds)
        {
            try
            {
                return await _context.MembershipRanks
                                     .Where(r => rankIds.Contains(r.Id))
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetRanksByIds: {ex.Message}");
                throw;
            }
        }

        public async Task<(List<Notification>, bool)> GetListNotificationSystemForPage(int page, int pageSize, string search)
        {
            _logger.LogInformation("Entered method GetListNotificationSystem in NotificationsRepository");

            try
            {
                var query = _context.Notifications
                    .Where(n => !n.IsDeleted && n.Type == (int)NotificationEnum.EType.System);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(n => n.Title.Contains(search));
                }
                query = query.OrderByDescending(n => n.CreatedAt);
                var totalItems = await query.CountAsync();
                var notifications = await query.Skip(page * pageSize).Take(pageSize).ToListAsync();
                bool hasMore = (page + 1) * pageSize < totalItems;

                _logger.LogInformation("Successfully retrieved {Count} notification system.", notifications.Count);
                return (notifications, hasMore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                throw;
            }
        }

        public async Task<(int userNotificationsCount, int systemNotificationsCount)> GetNotificationCounts()
        {
            _logger.LogInformation("Entered method GetNotificationCounts in NotificationsRepository");

            try
            {
                var userNotificationsCount = await _context.Notifications
                    .Where(n => !n.IsDeleted && n.Type == (int)NotificationEnum.EType.User)
                    .CountAsync();

                var systemNotificationsCount = await _context.Notifications
                    .Where(n => !n.IsDeleted && n.Type == (int)NotificationEnum.EType.System)
                    .CountAsync();

                return (userNotificationsCount, systemNotificationsCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting notifications.");
                throw;
            }
        }

        public async Task<int> GetNotificationsCountToday()
        {
            _logger.LogInformation("Entered method GetNotificationsCountToday in NotificationsRepository");

            try
            {
                var todayStart = DateTime.Today;
                var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);

                var count = await _context.Notifications
                    .Where(n => !n.IsDeleted && n.CreatedAt >= todayStart && n.CreatedAt <= todayEnd)
                    .CountAsync();

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting today's notifications.");
                throw;
            }
        }

        public async Task<List<Notification>> GetListNotificationSystemBase()
        {
            _logger.LogInformation("Entered method GetListNotificationSystemBase in NotificationsRepository");

            try
            {
                var notifications = await _context.Notifications
                    .Where(n => !n.IsDeleted && n.Type == (int)NotificationEnum.EType.System)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(6)
                    .ToListAsync();


                _logger.LogInformation("Successfully retrieved {Count} notification system .", notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                throw;
            }
        }



    }
}
