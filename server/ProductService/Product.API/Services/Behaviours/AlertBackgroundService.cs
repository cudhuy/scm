
using Product.API.Services.Interfaces;

namespace Product.API.Services.Behaviours
{
    public class AlertBackgroundService : BackgroundService
    {
        private readonly ILogger<AlertBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);

        public AlertBackgroundService(ILogger<AlertBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Alert Background Service is starting.");
            await Task.Delay(_interval, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting alert check...");
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();
                        await alertService.CheckAndSendAlerts();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking and sending alerts.");
                }

                _logger.LogInformation($"Alert check completed. Waiting for next run in {_interval.TotalHours} hours.");
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Alert Background Service is stopping.");
        }
    }
}
