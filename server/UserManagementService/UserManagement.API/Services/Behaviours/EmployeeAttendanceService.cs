using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using System.Net;
using System.Net.Sockets;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        private readonly ILogger<EmployeeAttendanceService> _logger;
        private readonly IEmployeeAttendanceRepository _employeeAttendanceRepository;
        private readonly IEmployeeShiftsRepository _employeeShiftsRepository;
        private readonly IMapper _mapper;

        public EmployeeAttendanceService(ILogger<EmployeeAttendanceService> logger, IEmployeeAttendanceRepository employeeAttendanceRepository, IMapper mapper, IEmployeeShiftsRepository employeeShiftsRepository)
        {
            _logger = logger;
            _employeeAttendanceRepository = employeeAttendanceRepository;
            _employeeShiftsRepository = employeeShiftsRepository;
            _mapper = mapper;
        }

        public async Task<AttendanceResponseDto> CheckIn(CheckInParameterDto input, HttpContext context)
        {
            _logger.LogInformation("Entered method CheckIn EmployeeAttendanceService");
            var result = new AttendanceResponseDto();
            var ipv4 = GetIpV4(context);
            var branch = await _employeeAttendanceRepository.GetBranchByEmployeeShiftId(input.EmployeeShiftId);
            var employeeShift = await _employeeAttendanceRepository.GetEmployeeShiftById(input.EmployeeShiftId);
            if (employeeShift != null && employeeShift.Status == nameof(EmployeeShiftEnum.EStatus.NotStarted)
                && branch != null && branch.IpAddress == ipv4)
            {
                employeeShift.StartAmount = input.StartAmount;
                this.DetermineShiftStatusAndCheckInDate(employeeShift);
                var checkInDateValid = (employeeShift.WorkDate.Date.Add(employeeShift.WorkShift.StartTime)).AddMinutes(-30);
                if (employeeShift.CheckInDate < checkInDateValid)
                {
                    result.Result = false;
                    result.Message = "Bạn chỉ được check-in tối đa 30 phút trước ca làm.";
                    return result;
                }
                result.Result = await _employeeAttendanceRepository.Save(); 
                result.Message = "Check in thành-công!";
                return result;
            }
            _logger.LogInformation("Entered method CheckIn EmployeeAttendanceService");
            result.Result = false;
            result.Message = "Không thể check-in!";
            return result;
        }

        public async Task<AttendanceResponseDto> CheckOut(CheckOutParameterDto input, HttpContext context)
        {
            _logger.LogInformation("Entered method CheckOut EmployeeAttendanceService");
            var result = new AttendanceResponseDto();
            var ipv4 = GetIpV4(context);
            var branch = await _employeeAttendanceRepository.GetBranchByEmployeeShiftId(input.EmployeeShiftId);
            var employeeShift = await _employeeAttendanceRepository.GetEmployeeShiftById(input.EmployeeShiftId);
            if (employeeShift != null && (employeeShift.Status == nameof(EmployeeShiftEnum.EStatus.Started) || employeeShift.Status == nameof(EmployeeShiftEnum.EStatus.Late))
                && branch != null && branch.IpAddress == ipv4)
            {
                employeeShift.EndAmount = input.EndAmount;
                this.DetermineShiftStatusAndCheckOutDate(employeeShift);
                var checkOutDateValid = (employeeShift.WorkDate.Date.Add(employeeShift.WorkShift.EndTime)).AddMinutes(-5);
                if (employeeShift.CheckOutDate < checkOutDateValid)
                {
                    result.Result = false;
                    result.Message = "Bạn chỉ được check-out tối đa 5 phút trước ca kết thúc.";
                    return result;
                }
                result.Result = await _employeeAttendanceRepository.Save();
                result.Message = "Check-out thành công!";
                return result;
            }
            _logger.LogInformation("Entered method CheckOut EmployeeAttendanceService");
            result.Result = false;
            result.Message = "Không thể check-out!";
            return result;
        }

        private void DetermineShiftStatusAndCheckInDate(EmployeeShift employeeShift)
        {
            _logger.LogInformation("Entered method ValidateStatus EmployeeAttendanceService");
            employeeShift.CheckInDate = DateTime.Now;
            var checkInDatetimeValid = employeeShift.WorkDate.Date.Add(employeeShift.WorkShift.StartTime)
                .AddMinutes(employeeShift.WorkShift.GracePeriod);
            if (employeeShift.CheckInDate > checkInDatetimeValid)
                employeeShift.Status = nameof(EmployeeShiftEnum.EStatus.Late);
            else
                employeeShift.Status = nameof(EmployeeShiftEnum.EStatus.Started);
            _logger.LogInformation("Leaving method ValidateStatus EmployeeAttendanceService");
        }

        private void DetermineShiftStatusAndCheckOutDate(EmployeeShift employeeShift)
        {
            _logger.LogInformation("Entered method ValidateStatus EmployeeAttendanceService");
            employeeShift.CheckOutDate = DateTime.Now;
            var checkOutDatetimeValid = employeeShift.WorkDate.Date.Add(employeeShift.WorkShift.EndTime)
                .AddMinutes(employeeShift.WorkShift.GracePeriod);
            if (employeeShift.CheckOutDate > checkOutDatetimeValid)
                employeeShift.Status = nameof(EmployeeShiftEnum.EStatus.NotComplete);
            else
                employeeShift.Status = nameof(EmployeeShiftEnum.EStatus.Completed);
            _logger.LogInformation("Leaving method ValidateStatus EmployeeAttendanceService");
        }

        public async Task<List<ScheduleDetail>> GetIndividualAttendance(Guid employeeId)
        {
            _logger.LogInformation("Entered method CheckIn GetIndividualAttendance");
            var result = new List<ScheduleDetail>();
            var entity = await _employeeAttendanceRepository.GetIndividualAttendance(employeeId);
            if (entity == null) return result;
            result = _mapper.Map<List<ScheduleDetail>>(entity);
            _logger.LogInformation("Leaving method CheckIn GetIndividualAttendance");
            return result;
        }

        private string GetIpV4(HttpContext context)
        {
            _logger.LogInformation("Entered method GetIpV4 EmployeeAttendanceService");

            try
            {
                // Lấy IP từ X-Forwarded-For nếu có
                var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    var forwardedIp = forwardedHeader.Split(',')[0].Trim(); // Lấy IP đầu tiên
                    _logger.LogInformation($"X-Forwarded-For IP: {forwardedIp}");
                    return forwardedIp;
                }

                // Nếu không có X-Forwarded-For, lấy từ RemoteIpAddress
                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                if (remoteIpAddress == null)
                {
                    _logger.LogWarning("Remote IP Address is null.");
                    return string.Empty;
                }

                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    try
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress)
                            .AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error resolving IPv6 to IPv4.");
                        return string.Empty;
                    }
                }

                string ipv4 = remoteIpAddress.ToString();
                _logger.LogInformation($"Resolved IP: {ipv4}");
                return ipv4;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetIpV4.");
                return string.Empty;
            }
        }

        //public string GetIpV4(HttpContext context)
        //{
        //    _logger.LogInformation("Entered method GetIpV4 EmployeeAttendanceService");

        //    try
        //    {
        //        IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
        //        if (remoteIpAddress == null)
        //        {
        //            _logger.LogWarning("Remote IP Address is null.");
        //            return string.Empty;
        //        }

        //        if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        //        {
        //            try
        //            {
        //                remoteIpAddress = Dns.GetHostEntry(remoteIpAddress)
        //                    .AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        //            }
        //            catch (SocketException ex)
        //            {
        //                _logger.LogError(ex, "Failed to resolve IPv6 to IPv4.");
        //                return string.Empty; // Return null if can not get IPv4
        //            }
        //            catch (InvalidOperationException ex)
        //            {
        //                _logger.LogError(ex, "No IPv4 address found for IPv6.");
        //                return string.Empty;
        //            }
        //        }

        //        string ipv4 = remoteIpAddress.ToString();
        //        _logger.LogInformation($"Resolved IP: {ipv4}");
        //        return ipv4;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error in GetIpV4.");
        //        return string.Empty;
        //    }
        //}

        public async Task<Guid> GetEmployeeIdByEmployeeShift(Guid employeeShiftId)
        {
            var entity = await _employeeShiftsRepository.GetEmployeeShiftById(employeeShiftId);
            return entity?.EmployeeId ?? Guid.Empty;
        }
    }
}
