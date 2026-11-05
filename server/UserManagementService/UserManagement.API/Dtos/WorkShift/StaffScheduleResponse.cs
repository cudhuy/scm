using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Dtos.WorkShift
{
    public class StaffScheduleResponse
    {
        public List<ScheduleDetail> ScheduleDetail { get; set; } = new List<ScheduleDetail>();
        public Guid BranchId { get; set; }
        public DateTime WorkDate { get; set; }


    }

    public class ScheduleDetail
    {
        public Guid EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public Guid WorkShiftId { get; set; }
        public Guid EmployeeShiftId { get; set; }

        public DateTime WorkDate { get; set; }

        public string Status { get; set; }

        public decimal StartAmount { get; set; }

        public decimal EndAmount { get; set; }

        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckOutDate { get; set; }
    }
}
