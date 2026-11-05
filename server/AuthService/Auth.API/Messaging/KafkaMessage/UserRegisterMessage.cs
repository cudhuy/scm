namespace Auth.API.Messaging.KafkaMessage
{
    public class UserRegisterMessage
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? Dob { get; set; }
        public bool IsVerified { get; set; }
        public string RoleId { get; set; } // Chuyển Guid -> string
        public string? Status { get; set; }
    }
}
