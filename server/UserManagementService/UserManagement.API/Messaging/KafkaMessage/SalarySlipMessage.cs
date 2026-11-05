namespace UserManagement.API.Messaging.KafkaMessage
{
    public class SalarySlipMessage
    {
        public string StaffName { get; set; }
        public string BranchName { get; set; }
        public string Email { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}
