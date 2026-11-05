namespace UserManagement.API.Messaging.KafkaMessage
{
    public class KafkaSettings
    {
        public string? BootstrapServers { get; set; }
        public string? GroupId { get; set; }
        public List<string>? ProducerTopic { get; set; } = new();
        public List<string>? ConsumerTopics { get; set; } = new();
    }
}
