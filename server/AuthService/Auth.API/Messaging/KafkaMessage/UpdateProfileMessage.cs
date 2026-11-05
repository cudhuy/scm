namespace Auth.API.Messaging.KafkaMessage
{
    public class UpdateProfileMessage
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public DateTime? Dob { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
