using AutoMapper;
using Domain.Entities;
using Newtonsoft.Json;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Helpers;
using UserManagement.API.Messaging.KafkaMessage;
using UserManagement.API.Messaging.Producers;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class NotificationsService : INotificationsService
    {
        private readonly ILogger<NotificationsService> _logger;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMapper _mapper;
        private readonly EmailHelper _emailHelper;
        private readonly SendNotificationEmailProducerService _sendNotificationEmailProducerService;

        public NotificationsService(ILogger<NotificationsService> logger, IMapper mapper, INotificationsRepository notificationsRepository, EmailHelper emailHelper, SendNotificationEmailProducerService sendNotificationEmailProducerService)
        {
            _mapper = mapper;
            _logger = logger;
            _notificationsRepository = notificationsRepository;
            _emailHelper = emailHelper;
            _sendNotificationEmailProducerService = sendNotificationEmailProducerService;
        }

        public async Task<IEnumerable<NotificationSystemDto>> GetAllNotificationSystem()
        {
            try
            {
                var notifications = await _notificationsRepository.GetListNotificationSystem();
                if (notifications == null || !notifications.Any())
                {
                    _logger.LogInformation("No notifications found.");
                    return Enumerable.Empty<NotificationSystemDto>();
                }

                var notificationDtos = notifications.Select(n => new NotificationSystemDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    //  Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    CreatedBy = n.CreatedBy,
                    // UpdatedAt = n.UpdatedAt,
                    //  UpdatedBy = n.UpdatedBy
                }).ToList();

                _logger.LogInformation($"{notificationDtos.Count} notifications found.NotificationsService");
                return notificationDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllNotificationSystem)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<IEnumerable<UserWithNotificationsDto>> GetAllNotificationUsers()
        {
            try
            {
                var notifications = await _notificationsRepository.GetListNotificationUser();
                if (notifications == null || !notifications.Any())
                {
                    _logger.LogInformation("No user notifications found.");
                    return Enumerable.Empty<UserWithNotificationsDto>();
                }

                var userNotificationDtos = notifications.Select(n => _mapper.Map<UserWithNotificationsDto>(n)).ToList();

                _logger.LogInformation($"{userNotificationDtos.Count} user notifications found.NotificationsService");
                return userNotificationDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllNotificationUsers)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }



        public async Task<bool> DeleteNotificationSystem(Guid id)
        {
            _logger.LogInformation("Entered method DeleteNotificationSystem in service for notification ID {Id}'NotificationsService'", id);

            try
            {
                var result = await _notificationsRepository.DeletetNotificationSystem(id);
                if (!result)
                {
                    _logger.LogWarning("Notification with ID {Id} not found in service.'NotificationsService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully marked notification with ID {Id} as deleted in service.'NotificationsService'", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(DeleteNotificationSystem)}: {ex.Message}, StackTrace: {ex.StackTrace}'NotificationsService'");
                throw;
            }
        }

        public async Task<bool> UpdateNotificationSystem(Guid id, UpdateNotificationSystemDto updateNotificationSystemDto)
        {

            _logger.LogInformation("Entered UpdateNotificationSystem  in service for role with id {id}.'NotificationsService'", id);

            try
            {
                var existingNotificationSystem = await _notificationsRepository.GetNotificationSystemById(id);
                if (existingNotificationSystem == null)
                {
                    _logger.LogWarning("notificationSystem with id {id} not found.'NotificationsService'", id);
                    return false;
                }


                var updatedNotifications = _mapper.Map<Notification>(updateNotificationSystemDto);

                var resultNotificationsUpdate = await _notificationsRepository.UpdateNotificationSystem(id, updatedNotifications);

                if (!resultNotificationsUpdate)
                {
                    _logger.LogWarning("Failed to update notification with id {Id}.'NotificationsService'", id);
                    return false;
                }

                _logger.LogInformation("Successfully updated notification with id {Id}.'NotificationsService'", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(UpdateNotificationSystem)}: {ex.Message}, StackTrace: {ex.StackTrace}.'NotificationsService'");
                throw;
            }
        }

        public async Task<bool> CreateNotificationSystem(CreateNotificationSystemDto createNotificationSystemDto)
        {
            _logger.LogInformation("Creating a new notification System. 'CreateNotificationSystem'");

            var notificationSystem = _mapper.Map<Notification>(createNotificationSystemDto);
            var result = await _notificationsRepository.CreateNotificationSystem(notificationSystem);

            if (!result)
            {
                _logger.LogWarning("Failed to create notification System.'CreateNotificationSystem'");
                return false;
            }

            _logger.LogInformation("Notification System created successfully.'CreateNotificationSystem'");
            return true;
        }


        public async Task<bool> CreateNotificationUser(CreateNotificationUserDto createNotificationUserDto)
        {
            _logger.LogInformation("Entered method CreateNotificationUser in NotificationsService");

            try
            {
                var userNotification = _mapper.Map<UserNotifications>(createNotificationUserDto);
                userNotification.Id = Guid.NewGuid();
                var result = await _notificationsRepository.CreateNotificationUser(userNotification);

                if (!result)
                {
                    _logger.LogWarning("Failed to create user notification.");
                    return false;
                }
                var email = await _notificationsRepository.GetUserEmailByIdAsync(createNotificationUserDto.UserId);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Do not found email user ");
                    return false;
                }
                var emailSubject = "Thông Báo Của Quán Coffee Chúng Tôi ";
                var emailBody = GenerateEmailBody(
                   createNotificationUserDto.Title,
                    $"Chào bạn, đã có thông báo mới từ quán cho bạn là : {createNotificationUserDto.Content} !"
                );

                var emailResult = await _emailHelper.SendEmail(email, emailSubject, emailBody);
                if (!emailResult)
                {
                    _logger.LogWarning("Không thể gửi email cho người dùng.");
                    return false;
                }

                _logger.LogInformation("Successfully created user notification and sent mail.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(CreateNotificationUser)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public string GenerateEmailBody(string title, string content)
        {
            return $@"
            <html>
            <head>
                <title>{title}</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .email-container {{
                        width: 100%;
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
                    }}
                    .email-header {{
                        text-align: center;
                        padding-bottom: 20px;
                    }}
                    .email-header h1 {{
                        color: #333333;
                        font-size: 24px;
                        margin: 0;
                    }}
                    .email-body {{
                        font-size: 16px;
                        color: #333333;
                        line-height: 1.5;
                        margin-top: 10px;
                    }}
                    .email-footer {{
                        text-align: center;
                        margin-top: 20px;
                        font-size: 14px;
                        color: #888888;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <div class='email-header'>
                        <h1>{title}</h1>
                    </div>
                    <div class='email-body'>
                        <p>{content}</p>
                    </div>
                    <div class='email-footer'>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                    </div>
                </div>
            </body>
            </html>
        ";
        }

        //public async Task<IEnumerable<RankWithNotificationsDto>> GetAllNotificationRanks()
        //{
        //    try
        //    {
        //        var notifications = await _notificationsRepository.GetListNotificationRank();
        //        if (notifications == null || !notifications.Any())
        //        {
        //            _logger.LogInformation("No notifications found.");
        //            return Enumerable.Empty<RankWithNotificationsDto>();
        //        }

        //        var rankNotifications = notifications
        //            .Where(n => n.User?.Ranks != null)
        //            .GroupBy(n => n.User.Ranks.Name)
        //            .Select(g => new RankWithNotificationsDto
        //            {
        //                RankName = g.Key,
        //                Notifications = g.Select(n => _mapper.Map<NotificationForRanksDto>(n.Notification)).ToList()
        //            })
        //            .ToList();

        //        _logger.LogInformation($"{rankNotifications.Count} rank-based notifications found.");
        //        return rankNotifications;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error in service layer {nameof(GetAllNotificationRanks)}: {ex.Message}, StackTrace: {ex.StackTrace}");
        //        throw;
        //    }
        //}

        public async Task<bool> CreateNotificationForRanks(CreateNotificationRanksDtos createNotificationRanksDtos)
        {
            _logger.LogInformation("Entered method CreateNotificationForRanks in NotificationsService");

            try
            {
                var ranksExist = await _notificationsRepository.GetRanksByIds(createNotificationRanksDtos.RankIds);
                if (ranksExist == null || ranksExist.Count() != createNotificationRanksDtos.RankIds.Count())
                {
                    _logger.LogWarning("One or more RankIds do not exist.");
                    return false;
                }

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = createNotificationRanksDtos.Title,
                    Content = createNotificationRanksDtos.Content,
                };

                var result = await _notificationsRepository.CreateNotificationForRank(notification);
                if (!result)
                {
                    _logger.LogWarning("Failed to create notification.");
                    return false;
                }

                var users = await _notificationsRepository.GetUsersByRank(createNotificationRanksDtos.RankIds);
                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found for the ranks.");
                    return false;
                }

                var userNotifications = new List<UserNotifications>();

                foreach (var user in users)
                {
                    var userNotification = new UserNotifications
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        NotificationId = notification.Id,
                        RankId = user.Ranks.Id


                    };

                    userNotifications.Add(userNotification);
                }

                var bulkResult = await _notificationsRepository.CreateBulkUserNotifications(userNotifications);
                if (!bulkResult)
                {
                    _logger.LogWarning("Failed to create user notifications.");
                    return false;
                }

                //send email to users
                foreach (var user in users)
                {
                    var email = await _notificationsRepository.GetUserEmailByIdAsync(user.Id);
                    if (string.IsNullOrEmpty(email))
                    {
                        _logger.LogWarning("Email of user with ID: {UserId} not found", user.Id);
                        continue;
                    }

                    var emailSubject = "Thông Báo Của Quán Coffee Chúng Tôi";
                    var emailBody = GenerateEmailBody(
                        createNotificationRanksDtos.Title,
                        $"Chào bạn, đã có thông báo mới từ quán cho bạn là: {createNotificationRanksDtos.Content}!"
                    );
                    var notificationEmail = new SendNotificationEmailMessage
                    {
                        Email = email,
                        EmailSubject = emailSubject,
                        EmailBody = emailBody
                    };

                    var messageJson = JsonConvert.SerializeObject(notificationEmail);
                    await _sendNotificationEmailProducerService.ProducerMessage(messageJson);
                    //var emailResult = await _emailHelper.SendEmail(email, emailSubject, emailBody);
                    //if (!emailResult)
                    //{
                    //    _logger.LogWarning("Cannot send email to user with email: {Email}", email);
                    //    continue;
                    //}
                    _logger.LogInformation("Data sent successfully to kafka with ID: {UserId} and email: {Email}", user.Id, email);
                }
                _logger.LogInformation("Successfully created notification and associated user notifications.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateNotificationForRanks: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<RankNotificationResponse> GetAllNotificationRanksForPage(int page, int pageSize, string search, List<Guid> rankIds)
        {
            try
            {
                var notifications = await _notificationsRepository.GetListNotificationRankForPage(page, pageSize, search, rankIds);

                if (!notifications.Any())
                {
                    _logger.LogInformation("No notifications found.");
                    return new RankNotificationResponse { Items = new List<NotificationForRanksDto>(), NextPage = null };
                }

                var distinctRankIds = notifications.Select(n => n.RankId).Distinct().ToList();
                var rankNames = await _notificationsRepository.GetRankNames(distinctRankIds);

                var notificationDtos = notifications.Select(n => new NotificationForRanksDto
                {
                    RankId = n.RankId,
                    RankName = rankNames.ContainsKey(n.RankId) ? rankNames[n.RankId] : string.Empty,
                    Title = n.Notification.Title,
                    Content = n.Notification.Content,
                    CreatedAt = n.Notification.CreatedAt
                }).ToList();

                _logger.LogInformation($"{notificationDtos.Count} notifications found.");

                bool hasMore = notifications.Count >= pageSize;
                int? nextPage = hasMore ? page + 1 : null;

                return new RankNotificationResponse { Items = notificationDtos, NextPage = nextPage };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetAllNotificationRanksForPage)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }






        public async Task<NotificationResponse> GetAllNotificationSystemForPage(int page, int pageSize, string search)
        {
            try
            {
                var (notifications, hasMore) = await _notificationsRepository.GetListNotificationSystemForPage(page, pageSize, search);

                if (!notifications.Any())
                {
                    _logger.LogInformation("No notifications found.");
                    return new NotificationResponse { Items = new List<NotificationSystemDto>(), NextPage = null };
                }

                var notificationDtos = notifications.Select(n => new NotificationSystemDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    CreatedAt = n.CreatedAt,
                    CreatedBy = n.CreatedBy,
                    Priority = n.Priority
                }).ToList();

                _logger.LogInformation($"{notificationDtos.Count} notifications found.");
                return new NotificationResponse { Items = notificationDtos, NextPage = hasMore ? page + 1 : null };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetAllNotificationSystemForPage)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<NotificationCountsDto> GetNotificationCounts()
        {
            try
            {
                var (userNotificationsCount, systemNotificationsCount) = await _notificationsRepository.GetNotificationCounts();
                var notificationsTodayCount = await _notificationsRepository.GetNotificationsCountToday();

                return new NotificationCountsDto
                {
                    UserNotificationsCount = userNotificationsCount,
                    SystemNotificationsCount = systemNotificationsCount,
                    TotalNotificationsCount = userNotificationsCount + systemNotificationsCount,
                    NotificationsTodayCount = notificationsTodayCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetNotificationCounts)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<IEnumerable<NotiSystemBaseDto>> GetNotificationSystemBase()
        {
            try
            {
                var notifications = await _notificationsRepository.GetListNotificationSystemBase();
                if (notifications == null || !notifications.Any())
                {
                    _logger.LogInformation("No notifications found.");
                    return Enumerable.Empty<NotiSystemBaseDto>();
                }

                var notificationBaseDto = notifications.Select(n => new NotiSystemBaseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Priority = n.Priority
                }).ToList();

                _logger.LogInformation($"{notificationBaseDto.Count} notifications found.NotificationsService");
                return notificationBaseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in service layer {nameof(GetAllNotificationSystem)}: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
