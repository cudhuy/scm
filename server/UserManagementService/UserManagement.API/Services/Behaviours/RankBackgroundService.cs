using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class RankBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RankBackgroundService> _logger;

        public RankBackgroundService(IServiceScopeFactory scopeFactory, ILogger<RankBackgroundService> logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_scopeFactory == null)
            {
                _logger.LogError("_scopeFactory is NULL! Background service cannot start.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Background Service Running...");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var rankUpdateService = scope.ServiceProvider.GetService<IRankUpdateService>();

                        if (rankUpdateService == null)
                        {
                            _logger.LogError("IRankUpdateService is not registered in the DI container.");
                            throw new InvalidOperationException("IRankUpdateService is missing from dependency injection.");
                        }

                        await rankUpdateService.UpdateUserRanksAsync();
                    }

                    _logger.LogInformation("User ranks updated successfully.");
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Background service stopping due to cancellation.");
                    break;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Operation was canceled.");
                    break;
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogCritical(ex, "Critical configuration error: {Message}", ex.Message);
                    break; 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occurred while updating user ranks.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("Background service stopped.");
        }

    }
}
//