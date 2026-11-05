
using Confluent.Kafka;
using Microsoft.IdentityModel.Tokens;
using UserManagement.API.Messaging.KafkaMessage;

namespace UserManagement.API.Messaging.Producers
{
    public class CreateStaffProducerService
    {
        private readonly ILogger<CreateStaffProducerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly KafkaSettings _kafkaSettings;

        public CreateStaffProducerService(ILogger<CreateStaffProducerService> logger, IConfiguration configuration, KafkaSettings kafkaSettings)
        {
            _logger = logger;
            _configuration = configuration;
            _kafkaSettings = kafkaSettings;
        }

        public async Task ProducerMessage(string message)
        {
            _logger.LogInformation("Entered method ProducerMessage");
            var config = new ProducerConfig { BootstrapServers = _kafkaSettings.BootstrapServers, Acks = Acks.All, MessageSendMaxRetries = 0, SecurityProtocol = SecurityProtocol.Plaintext };
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                // Topic: create-staff-topic
                var response = await producer.ProduceAsync(_kafkaSettings.ProducerTopic[0],
                        new Message<Null, string> { Value = message });
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex.Message);
            }
            _logger.LogInformation("Leaving method ProducerMessage");
        }
    }
}
