using Confluent.Kafka;
using UserManagement.API.Messaging.KafkaMessage;

namespace UserManagement.API.Messaging.Producers
{
    public class SendNotificationEmailProducerService
    {
        private readonly ILogger<SendNotificationEmailProducerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly KafkaSettings _kafkaSettings;

        public SendNotificationEmailProducerService(ILogger<SendNotificationEmailProducerService> logger, IConfiguration configuration, KafkaSettings kafkaSettings)
        {
            _logger = logger;
            _configuration = configuration;
            _kafkaSettings = kafkaSettings;
        }

        public async Task ProducerMessage(string message)
        {
            _logger.LogInformation("Entered method ProducerMessage SendNotificationEmailService");
            var config = new ProducerConfig { BootstrapServers = _kafkaSettings.BootstrapServers, Acks = Acks.All, MessageSendMaxRetries = 0, SecurityProtocol = SecurityProtocol.Plaintext };
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                // Topic: notification-email-topic
                var response = await producer.ProduceAsync(_kafkaSettings.ProducerTopic[2],
                    new Message<Null, string> { Value = message });
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex.Message);
            }
            _logger.LogInformation("Leaving method ProducerMessage SendNotificationEmailService");
        }
    }
}
