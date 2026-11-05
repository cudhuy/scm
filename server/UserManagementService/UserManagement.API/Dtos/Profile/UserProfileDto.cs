namespace UserManagement.API.Dtos.Profile
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Dob { get; set; }
        public string Address { get; set; }
        public bool IsVerified { get; set; }
      //  public string RoleName { get; set; }
        public string RankName { get; set; }
        public string Avatar { get; set; }
        public string Status { get; set; }
        public int TotalOrders { get; set; }       
        public int PendingOrders { get; set; }  
        public int CompletedOrders { get; set; }
    }
}
