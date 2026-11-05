using Confluent.Kafka;
using Newtonsoft.Json;
using Domain.Constanst;
using Domain.Context;
using Domain.Entities;
using UserManagement.API.Messaging.KafkaMessage;
using UserManagement.API.Helpers;

namespace UserManagement.API.Messaging.Consumers
{
    public class ConsumerBackgroundService : BackgroundService
    {
        private readonly ILogger<ConsumerBackgroundService> _logger;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConsumerConfig _config;

        public ConsumerBackgroundService(IServiceProvider serviceProvider, ILogger<ConsumerBackgroundService> logger, KafkaSettings kafkaSettings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _kafkaSettings = kafkaSettings;
            _config = new ConsumerConfig
            {
                GroupId = _kafkaSettings.GroupId,
                BootstrapServers = _kafkaSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Entered method ExecuteAsync in ConsumerBackgroundService");
            return Task.Run(async () =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
                consumer.Subscribe(_kafkaSettings.ConsumerTopics);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(stoppingToken);

                        // Tạo Scope một lần cho cả DbContext và EmailHelper
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var serviceProvider = scope.ServiceProvider;
                            var dbContext = serviceProvider.GetRequiredService<SCMDbContext>();
                            var emailHelper = serviceProvider.GetRequiredService<EmailHelper>();

                            await ProcessMessage(consumeResult.Topic, consumeResult.Message.Value, dbContext, emailHelper);
                        }

                        _logger.LogInformation($"Received message: {consumeResult.Message.Value}");
                        consumer.Commit(consumeResult);
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
                _logger.LogInformation("Leaving method ExecuteAsync in ConsumerBackgroundService");
            }, stoppingToken);
        }

        private async Task ProcessMessage(string topic, string message, SCMDbContext dbContext, EmailHelper emailHelper)
        {
            _logger.LogInformation("Entered method ProcessMessage in ConsumerBackgroundService");

            try
            {
                switch (topic)
                {
                    case "user-register-topic":
                        _logger.LogInformation("Processing user-register-topic");
                        var user = JsonConvert.DeserializeObject<User>(message);
                        if (user != null)
                        {
                            user.RankId = AppConstants.MEMBERSHIP_REGULARS_GUID;
                            await dbContext.Users.AddAsync(user);
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            _logger.LogWarning("Failed to deserialize User object from message.");
                        }
                        break;

                    case "salary-slip-email-topic":
                        _logger.LogInformation("Processing salary-slip-email-topic");
                        await emailHelper.SendEmailWithAttachment(message);
                        break;
                    case "notification-email-topic":
                        _logger.LogInformation("Processing notification-email-topic");
                        var notificationEmail = JsonConvert.DeserializeObject<SendNotificationEmailMessage>(message);
                        if (notificationEmail != null)
                            await emailHelper.SendEmail(notificationEmail.Email, notificationEmail.EmailSubject, notificationEmail.EmailBody);
                        break;
                    default:
                        _logger.LogWarning($"Received message from unknown topic: {topic}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message from topic {topic}: {ex.Message}");
            }

            _logger.LogInformation("Leaving method ProcessMessage in ConsumerBackgroundService");
        }
    }
}
