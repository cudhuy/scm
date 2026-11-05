namespace Auth.API.Messaging.KafkaMessage
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string ProducerTopic { get; set; } = string.Empty;
        public List<string> ConsumerTopics { get; set; } = new List<string>();
    }
}
