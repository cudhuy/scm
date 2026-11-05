using Auth.API.Messaging.KafkaMessage;
using Confluent.Kafka;

namespace Auth.API.Messaging.Producers
{
    public class RegisterProducerService
    {
        private readonly ILogger<RegisterProducerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly KafkaSettings _kafkaSettings;
        public RegisterProducerService(ILogger<RegisterProducerService> logger, IConfiguration configuration, KafkaSettings kafkaSettings)
        {
            _logger = logger;
            _configuration = configuration;
            _kafkaSettings = kafkaSettings;
        }
        public async Task ProducerMessage(string message)
        {
            _logger.LogInformation("Entered method ProducerMessage");
            //var kafkaConfig = _configuration.GetSection("Kafka").Get<KafkaSettings>();
            var config = new ProducerConfig { BootstrapServers = _kafkaSettings.BootstrapServers, Acks = Acks.All, MessageSendMaxRetries = 0, SecurityProtocol = SecurityProtocol.Plaintext };
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                var response = await producer.ProduceAsync(_kafkaSettings.ProducerTopic,
                    new Message<Null, string> { Value = message });
            }catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex.Message);
            }
            _logger.LogInformation("Leaving method ProducerMessage");
        }
    }
}
