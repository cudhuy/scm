namespace UserManagement.API.Dtos.Staff
{
    public class StaffInfoDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Dob { get; set; }
        public string Avatar { get; set; }
        public string? Status { get; set; }
        // public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        //public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public Guid? UpdatedBy { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public decimal SalaryBase { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string TypeBranch { get; set; }
        public bool IsDeleted { get; set; }
    }
}
