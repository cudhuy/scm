namespace UserManagement.API.Dtos.Staff
{
    public class StaffDetailDto
    {
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DOB { get; set; } 
        public string? Address { get; set; }
        public string? RoleName { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        public string? Title { get; set; }
        public string? Type { get; set; }  
        public DateTime? CreateAt { get; set; }
        public decimal SalaryBase { get; set; }
        public string? BranchName { get; set; }
    }
}
