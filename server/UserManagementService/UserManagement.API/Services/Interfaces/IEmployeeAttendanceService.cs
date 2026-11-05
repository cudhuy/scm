using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Services.Interfaces
{
    public interface IEmployeeAttendanceService
    {
        Task<AttendanceResponseDto> CheckIn(CheckInParameterDto input, HttpContext httpContext);
        Task<AttendanceResponseDto> CheckOut(CheckOutParameterDto input, HttpContext httpContext);
        Task<Guid> GetEmployeeIdByEmployeeShift(Guid employeeShiftId);
        Task <List<ScheduleDetail>> GetIndividualAttendance(Guid employeeId); 
    }
}
