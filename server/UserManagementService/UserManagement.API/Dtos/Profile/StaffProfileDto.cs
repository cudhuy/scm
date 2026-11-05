namespace UserManagement.API.Dtos.Profile
{
    public class StaffProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Tittle { get; set; }
        public string Type { get; set; }
        public decimal SalaryBase { get; set; }
        public DateTime? Dob { get; set; }
        public string Address { get; set; }
        public string BranchName { get; set; }

        public bool IsVerified { get; set; }
        public string RoleName { get; set; }
      //  public string RankName { get; set; }
        public string Avatar { get; set; }
        public string Status { get; set; }
    }
}
