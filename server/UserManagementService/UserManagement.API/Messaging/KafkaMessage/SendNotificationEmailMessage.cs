namespace UserManagement.API.Messaging.KafkaMessage
{
    public class SendNotificationEmailMessage
    {
        public string Email { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
}
