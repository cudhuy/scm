namespace UserManagement.API.Messaging.KafkaMessage
{
    public class StaffCreatedMessage
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public Guid BranchId { get; set; }
        public Guid RoleId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Status { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string? Phone { get; set; }
    }
}
