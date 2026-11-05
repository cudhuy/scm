using AutoMapper;
using Domain.Entities;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;

namespace UserManagement.API.Services.Behaviours
{
    public class WorkShiftsService : IWorkShiftsService
    {
        private readonly ILogger<WorkShiftsService> _logger;
        private readonly IWorkShiftsRepository _workShiftsRepository;
        private readonly IMapper _mapper;

        public WorkShiftsService(ILogger<WorkShiftsService> logger, IWorkShiftsRepository workShiftsRepository, IMapper mapper)
        {
            _logger = logger;
            _workShiftsRepository = workShiftsRepository;
            _mapper = mapper;
        }

        public async Task<bool> UpdateShift(UpdateWorkShiftDto workShift)
        {
            _logger.LogInformation("Entered method UpdateShift WorkShiftsService");
            var check = ValidateShift(workShift);
            if (!check) return false;
            var duration = workShift.EndTime - workShift.StartTime;
            var entity = await _workShiftsRepository.GetShiftById(workShift.Id);
            if (entity == null)
            {
                _logger.LogInformation("Shift not exist!");
                return false;
            }
            entity.StartTime = workShift.StartTime;
            entity.EndTime = workShift.EndTime;
            entity.Duration = duration;
            entity.GracePeriod = workShift.GracePeriod;
            var result = await _workShiftsRepository.Save();
            _logger.LogInformation("Leaving method UpdateShift WorkShiftsService");
            return result;
        }

        private bool ValidateShift(UpdateWorkShiftDto workShift)
        {
            if (workShift.EndTime <= workShift.StartTime)
            {
                workShift.EndTime = workShift.EndTime.Add(new TimeSpan(24, 0, 0));
            }

            if (workShift.GracePeriod < 0)
            {
                _logger.LogWarning("Grace Period must be greater than zero!");
                return false;
            }

            return true;
        }
        public async Task<List<WorkShiftResponseDto>> GetShifts()
        {
            _logger.LogInformation("Entered method GetShifts WorkShiftsService");
            var entity = await _workShiftsRepository.GetShifts();
            if (entity == null) return new List<WorkShiftResponseDto>();
            var result = _mapper.Map<List<WorkShift>, List<WorkShiftResponseDto>>(entity);
            _logger.LogInformation("Leaving method GetShifts WorkShiftsService");
            return result;
        }
    }
}
