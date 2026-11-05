namespace Auth.API.Dtos
{
    public class AuthResponseDto
    {
        public string Id { get; set; } // Guid -> string
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; } = "";
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; } = "";
        public string RoleName { get; set; }
        public string BranchId { get; set; } // Guid -> string
    }
}
