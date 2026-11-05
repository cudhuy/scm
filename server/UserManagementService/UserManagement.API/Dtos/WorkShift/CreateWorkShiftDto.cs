namespace UserManagement.API.Dtos.WorkShift
{
    public class UpdateWorkShiftDto
    {
        public Guid Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GracePeriod { get; set; } 
    }
}
