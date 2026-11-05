namespace UserManagement.API.Dtos.WorkShift
{
    public class WorkShiftResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GracePeriod { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
