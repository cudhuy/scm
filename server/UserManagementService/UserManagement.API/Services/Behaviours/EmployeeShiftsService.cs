using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class EmployeeShiftsService : IEmployeeShiftsService
    {
        private readonly ILogger<EmployeeShiftsService> _logger;
        private readonly IEmployeeShiftsRepository _employeeShiftsRepository;
        private readonly IMapper _mapper;

        public EmployeeShiftsService(ILogger<EmployeeShiftsService> logger, IEmployeeShiftsRepository employeeShiftsRepository, IMapper mapper)
        {
            _logger = logger;
            _employeeShiftsRepository = employeeShiftsRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddEmployeeShift(CreateEmployeeShiftDto input)
        {
            _logger.LogInformation("Entered method AddEmployeeShift EmployeeShiftsService");
            if (input.WorkDate.Date > DateTime.Now.Date.AddDays(7))
            {
                _logger.LogInformation("WorkDate ({WorkDate}) is beyond 7 days.", input.WorkDate.Date);
                return false;
            }
            input.WorkDate = input.WorkDate.Date;
            var entity = _mapper.Map<CreateEmployeeShiftDto, EmployeeShift>(input);
            entity.Status = EmployeeShiftEnum.EStatus.NotStarted.ToString();
            var result = await _employeeShiftsRepository.AddEmployeeShift(entity);
            _logger.LogInformation("Leaving method AddEmployeeShift EmployeeShiftsService");
            return result;
        }

        public async Task<bool> DeleteEmployeeShift(Guid id)
        {
            _logger.LogInformation("Entered method DeleteEmployeeShift EmployeeShiftsService");
            var entity = await _employeeShiftsRepository.GetEmployeeShiftById(id);
            if (entity != null)
                return await _employeeShiftsRepository.DeleteEmployeeShift(entity);
            _logger.LogInformation("Entered method DeleteEmployeeShift EmployeeShiftsService");
            return false;
        }

        public async Task<List<StaffInShift>> GetStaffs(Guid branchId)
        {
            _logger.LogInformation("Entered method GetStaffs EmployeeShiftsService");
            var result = new List<StaffInShift>();
            var entity = await _employeeShiftsRepository.GetStaffs(branchId);
            if (entity == null) return result;
            result = _mapper.Map<List<User>, List<StaffInShift>>(entity);
            _logger.LogInformation("Leaving method GetStaffs EmployeeShiftsService");
            return result;
        }

        public async Task<StaffScheduleResponse> GetStaffScheduleByDate(StaffScheduleParameter input)
        {
            _logger.LogInformation("Entered method GetStaffScheduleByDate EmployeeShiftsService");
            var result = new StaffScheduleResponse();
            var entity = await _employeeShiftsRepository.GetStaffScheduleByDate(input);
            if (entity == null) return result;
            var scheduleDetails = _mapper.Map<List<ScheduleDetail>>(entity);
            result.BranchId = input.BranchId;
            result.ScheduleDetail = scheduleDetails;
            _logger.LogInformation("Leaving method GetStaffScheduleByDate EmployeeShiftsService");
            return result;
        }

        public async Task<List<StaffScheduleResponse>> GetStaffSchedulesLastMonth(Guid branchId)
        {
            _logger.LogInformation("Entered method GetStaffSchedulesLastMonth EmployeeShiftsService");
            var result = new List<StaffScheduleResponse>();
            var entity = await _employeeShiftsRepository.GetStaffSchedulesLastMonth(branchId);
            if (entity == null) return result;
            var scheduleDetails = _mapper.Map<List<ScheduleDetail>>(entity);
            var scheduleDetailsGrouped = scheduleDetails.GroupBy(item => item.WorkDate.Date).ToList();
            result = scheduleDetailsGrouped.Select(group => new StaffScheduleResponse
            {
                BranchId = branchId,
                ScheduleDetail = group.ToList(),
                WorkDate = group.First().WorkDate,
            }).ToList();
            _logger.LogInformation("Leaving method GetStaffSchedulesLastMonth EmployeeShiftsService");
            return result;
        }
    }
}
