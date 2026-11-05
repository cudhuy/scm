using System.Data.Common;
using UserManagement.API.Dtos.Notification;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class RankUpdateService : IRankUpdateService
    {
        private readonly IMembershipRanksRepository _membershipRanksRepository;
        private readonly ILogger<RankUpdateService> _logger;
        private readonly INotificationsService _notificationsService;


        public RankUpdateService(IMembershipRanksRepository membershipRanksRepository, ILogger<RankUpdateService> logger, INotificationsService notificationsService)
        {
            _membershipRanksRepository = membershipRanksRepository;
            _logger = logger;
            _notificationsService = notificationsService;

        }


        public async Task UpdateUserRanksAsync()
        {
            try
            {
                _logger.LogInformation("Starting user rank update process.");

                var users = await _membershipRanksRepository.GetUsersWithOrdersAsync();
                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found for rank update.");
                    return;
                }

                foreach (var user in users)
                {
                    try
                    {
                        decimal totalAmount = await _membershipRanksRepository.GetTotalAmountByUserIdAsync(user.Id);
                        _logger.LogInformation("User {UserId} - Total Amount: {TotalAmount}", user.Id, totalAmount);

                        var highestRank = await _membershipRanksRepository.GetHighestEligibleRankAsync(totalAmount);
                        if (highestRank == null)
                        {
                            _logger.LogWarning("No valid rank found for User {UserId}. Skipping...", user.Id);
                            continue;
                        }

                        _logger.LogInformation("Highest Rank for User {UserId}: {RankId}", user.Id, highestRank.Id);

                        if (user.RankId != highestRank.Id)
                        {
                            await _membershipRanksRepository.UpdateUserRankAsync(user.Id, highestRank.Id);
                            _logger.LogInformation("Updated User {UserId} to Rank {RankId}", user.Id, highestRank.Id);
                            await SendRankUpgradeEmail(user.Id, highestRank.Name);
                        }
                        else
                        {
                            _logger.LogInformation("User {UserId} already has the highest eligible rank {RankId}. No update needed.", user.Id, highestRank.Id);
                        }
                    }
                    catch (DbException dbEx)
                    {
                        _logger.LogError(dbEx, "Database error while updating rank for User {UserId}", user.Id);
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogWarning(ex, "Invalid data for User {UserId}. Skipping...", user.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error while updating rank for User {UserId}", user.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in UpdateUserRanksAsync");
            }
            _logger.LogInformation("User rank update process completed.");
        }

        private async Task SendRankUpgradeEmail(Guid userId, string newRank)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid userId provided for rank upgrade email.");
                    return;
                }

                var email = await _membershipRanksRepository.GetUserEmailByIdAsync(userId);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("User {UserId} does not have an email. Skipping notification update ranks.", userId);
                    return;
                }
                if (string.IsNullOrWhiteSpace(newRank))
                {
                    _logger.LogWarning("User {UserId} does not have a valid rank. Skipping notification update ranks.", userId);
                    return;
                }
                var notificationDto = new CreateNotificationUserDto
                {
                    UserId = userId,
                    Title = "Chúc mừng bạn đã thăng hạng thành viên!",
                    Content = $"Xin chúc mừng! Bạn đã đạt được hạng thành viên mới: {newRank.ToUpper()} !" +
                    $" Cảm ơn bạn đã đồng hành cùng chúng tôi. Chúng tôi rất trân trọng sự ủng hộ của bạn!" +
                    $"Và đừng quên khám phá các ưu đãi và đặc quyền mới dành riêng cho hạng rank này.  \r\n        Chúc bạn có những trải nghiệm tuyệt vời cùng chúng tôi!"
                };

                bool success;
                try
                {
                    success = await _notificationsService.CreateNotificationUser(notificationDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi gửi thông báo thăng hạng cho User {UserId}.", userId);
                    return;
                }

                if (success)
                {
                    _logger.LogInformation("Successfully sent rank upgrade email to User {UserId}.", userId);
                }
                else
                {
                    _logger.LogWarning("Failed to send rank upgrade email to User {UserId}.", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending rank upgrade email to User {UserId}.", userId);
            }
        }


    }
}
