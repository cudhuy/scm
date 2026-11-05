using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Interfaces;
using UserManagement.API.Messaging.KafkaMessage;
using UserManagement.API.Messaging.Producers;
using Newtonsoft.Json;
using FluentValidation;
using FluentValidation.Results;

namespace UserManagement.API.Services.Behaviours
{
    public class SalaryService : ISalaryService
    {
        private readonly ILogger<SalaryService> _logger;
        private readonly ISalaryRepository _salaryRepository;
        private readonly IEmployeeShiftsRepository _employeeShiftsRepository;
        private readonly IStaffInformationRepository _staffInformationRepository;
        private readonly IMapper _mapper;
        private readonly SendSalarySlipProducerService _sendSalarySlipProducerService;
        private readonly IValidator<CreateMonthlySalaryDto> _validator;
        private readonly IWebHostEnvironment _env;

        public SalaryService(ILogger<SalaryService> logger,
            ISalaryRepository salaryRepository,
            IEmployeeShiftsRepository employeeShiftsRepository,
            IStaffInformationRepository staffInformationRepository,
            IMapper mapper,
            SendSalarySlipProducerService sendSalarySlipProducerService,
            IValidator<CreateMonthlySalaryDto> validator,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _salaryRepository = salaryRepository;
            _employeeShiftsRepository = employeeShiftsRepository;
            _staffInformationRepository = staffInformationRepository;
            _mapper = mapper;
            _sendSalarySlipProducerService = sendSalarySlipProducerService;
            _validator = validator;
            _env = env;
        }

        public async Task<List<MonthlySalaryResponse>> GetSalaryList(GetSalaryDto salaryDto)
        {
            try
            {
                _logger.LogInformation("Entered method GetSalaryList SalaryService");
                var salaryList = await _salaryRepository.GetSalaryList(salaryDto);
                if (!salaryList.Any())
                {

                    var result = new List<MonthlySalaryResponse>();

                    // Get all employee shifts
                    var employeeShift = await _employeeShiftsRepository.GetEmployeeShiftByDateAndBranch(salaryDto);

                    // Group by EmployeeId
                    var employeeShiftGrouped = employeeShift.GroupBy(item => item.EmployeeId).ToList();

                    // Get employee information
                    var staffInfoDict = await GetStaffInfoDictionary(employeeShiftGrouped);

                    foreach (var item in employeeShiftGrouped)
                    {
                        if (!staffInfoDict.TryGetValue(item.Key, out var staffInfo))
                            continue;

                        var response = CreateSalaryResponse(item, salaryDto, staffInfo);
                        result.Add(response);
                    }
                    _logger.LogInformation("Leaving method GetSalaryList SalaryService");
                    return result;
                }
                _logger.LogInformation("Leaving method GetSalaryList SalaryService");
                return _mapper.Map<List<Salary>, List<MonthlySalaryResponse>>(salaryList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error while GetSalaryList, {ex}");
                return new List<MonthlySalaryResponse>();
            }
        }

        private async Task<Dictionary<Guid, StaffInfo>> GetStaffInfoDictionary(List<IGrouping<Guid, EmployeeShift>> employeeShiftGrouped)
        {
            _logger.LogInformation("Entered method GetStaffInfoDictionary SalaryService");
            var employeeIds = employeeShiftGrouped.Select(g => g.Key).ToList();
            var staffInfos = await _staffInformationRepository.GetByUserIds(employeeIds);
            _logger.LogInformation("Entered method GetStaffInfoDictionary SalaryService");
            return staffInfos.ToDictionary(s => s.UserId, s => s);
        }

        private MonthlySalaryResponse CreateSalaryResponse(IGrouping<Guid, EmployeeShift> item, GetSalaryDto salaryDto, StaffInfo staffInfo)
        {
            _logger.LogInformation("Entered method CreateSalaryResponse SalaryService");
            var response = new MonthlySalaryResponse
            {
                Id = Guid.NewGuid(),
                Month = salaryDto.Month,
                Year = salaryDto.Year,
                BranchId = salaryDto.BranchId,
                EmployeeId = staffInfo.UserId,
                EmployeeName = staffInfo.User.FullName,
                SalaryBase = staffInfo.SalaryBase,
                TotalNumberOfShifts = item.Count()
            };
            response.TotalHours = CalculateTotalHours(item);
            int totalLateShifts, totalAbsentShifts, totalNotCompleteShifts;
            CountShiftStatus(item, out totalLateShifts, out totalAbsentShifts, out totalNotCompleteShifts);

            response.TotalLateShifts = totalLateShifts;
            response.TotalAbsentShifts = totalAbsentShifts;
            response.TotalNotCompleteShifts = totalNotCompleteShifts;
            // Calculate salary
            response.NetSalary = Convert.ToDecimal(response.TotalHours) * staffInfo.SalaryBase;
            _logger.LogInformation("Leaving method CreateSalaryResponse SalaryService");
            return response;
        }

        private double CalculateTotalHours(IGrouping<Guid, EmployeeShift> item)
        {
            var validShifts = item.Where(x => x.Status != nameof(EmployeeShiftEnum.EStatus.Absent));
            var shiftGrouped = validShifts.GroupBy(x => x.WorkShiftId).ToList();
            return shiftGrouped.Sum(group =>
            {
                var duration = group.First().WorkShift.Duration.TotalHours;
                return group.Count() * duration;
            });
        }

        private void CountShiftStatus(IGrouping<Guid, EmployeeShift> item, out int totalLateShifts, out int totalAbsentShifts, out int totalNotCompleteShifts)
        {
            totalLateShifts = 0;
            totalAbsentShifts = 0;
            totalNotCompleteShifts = 0;

            foreach (var shift in item)
            {
                switch (shift.Status)
                {
                    case nameof(EmployeeShiftEnum.EStatus.Late):
                        totalLateShifts++;
                        break;
                    case nameof(EmployeeShiftEnum.EStatus.Absent):
                        totalAbsentShifts++;
                        break;
                    case nameof(EmployeeShiftEnum.EStatus.NotComplete):
                        totalNotCompleteShifts++;
                        break;
                }
            }
        }

        public async Task<bool> ConfirmMonthlySalary(List<CreateMonthlySalaryDto> request)
        {
            try
            {
                _logger.LogInformation("Entered method ConfirmMonthlySalary SalaryService");
                foreach (var item in request)
                {
                    ValidationResult result = _validator.Validate(item);
                    if (!result.IsValid)
                    {
                        string errors = string.Join("; ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                        _logger.LogError($"Validation errors: {errors}");
                        return false;
                    }
                }
                _logger.LogInformation("Leaving method ConfirmMonthlySalary SalaryService");
                var entities = _mapper.Map<List<CreateMonthlySalaryDto>, List<Salary>>(request);
                return await _salaryRepository.AddSalaryList(entities);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Send Salary Slip
        public async Task<bool> SendSalarySlip(SendSalarySlipDto request)
        {
            try
            {
                _logger.LogInformation("Entered method SendSalarySlip SalaryService");
                var salarySlipList = await _salaryRepository.GetSalarySlip(request.SalarySlipIds);
                foreach (var item in salarySlipList)
                {
                    var salarySlipByte = await CreateFile(item);
                    var staffInfoMessage = new SalarySlipMessage
                    {
                        StaffName = item.Employee.FullName,
                        BranchName = item.Branch.Name,
                        Email = item.Employee.Email,
                        Month = item.Month,
                        Year = item.Year,
                        FileName = $"{item.Employee.FullName}_{item.Branch.Name}_T{item.Month}/{item.Year}",
                        Data = salarySlipByte
                    };
                    var messageJson = JsonConvert.SerializeObject(staffInfoMessage);
                    await _sendSalarySlipProducerService.ProducerMessage(messageJson);
                    //string filePath = Path.Combine("wwwroot", $"Luong_{item.EmployeeId}.pdf");
                    //await File.WriteAllBytesAsync(filePath, salarySlipByte);
                }
                _logger.LogInformation("Leaving method SendSalarySlip SalaryService");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<byte[]> CreateFile(Salary salarySlip)
        {
            // Non-generic Task so await at call site stays warning-free
            await Task.CompletedTask;
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A5);
                        page.MarginVertical(30);
                        page.MarginHorizontal(25);
                        page.Content().Column(col =>
                        {
                            col.Item().AlignCenter().Text("PHIẾU LƯƠNG NHÂN VIÊN").FontSize(16).Bold();
                            col.Item().PaddingTop(10).AlignCenter().Text($"Kỳ lương: Tháng {salarySlip.Month}/{salarySlip.Year}").FontSize(12).SemiBold();

                            col.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(6);
                                });
                                table.Header(header =>
                                {
                                    header.Cell().Background("#DCDCDC").Padding(5).AlignCenter().Text("Thông tin").SemiBold();
                                    header.Cell().Background("#DCDCDC").Padding(5).AlignCenter().Text("Giá trị").SemiBold();
                                });

                                void AddRow(string label, string value)
                                {
                                    table.Cell().Padding(5).AlignCenter().Text(label);
                                    table.Cell().Padding(5).AlignCenter().Text(value);
                                }

                                AddRow("Tên Nhân Viên", salarySlip.Employee.FullName);
                                AddRow("Lương Cơ Bản", $"{salarySlip.SalaryBase:N0} VND");
                                AddRow("Số Giờ Làm", $"{salarySlip.TotalHours}");
                                AddRow("Số Ca Làm", $"{salarySlip.TotalNumberOfShifts}");
                                AddRow("Số Ca Đến Muộn", $"{salarySlip.TotalLateShifts}");
                                AddRow("Số Ca Nghỉ", $"{salarySlip.TotalAbsentShifts}");
                                AddRow("Số Ca Không Hoàn Thành", $"{salarySlip.TotalNotCompleteShifts}");
                                AddRow("Thưởng", $"{salarySlip.Bonus:N0} VND");
                                AddRow("Phạt", $"{salarySlip.Penalty:N0} VND");
                                AddRow("Tổng Lương", $"{salarySlip.NetSalary:N0} VND");
                            });

                            // Thêm ngày lập phiếu
                            col.Item().PaddingTop(10).AlignRight().Text($"Ngày lập phiếu: {DateTime.Now:dd/MM/yyyy}");

                            // Thêm ghi chú hoặc lời nhắn
                            col.Item().PaddingTop(10).AlignCenter().Text("Cảm ơn bạn đã làm việc chăm chỉ! Chúc bạn một ngày tốt lành!").FontSize(10).Italic();
                        });

                        // Thêm tên quán vào góc dưới bên phải
                        page.Footer().AlignRight().Text(text =>
                        {
                            text.AlignRight();
                            text.DefaultTextStyle(x => x.FontSize(10).Italic());
                            text.Span("Xóm Cafe");
                        });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while creating file: {0}", ex);
                throw new Exception("An error occurred while creating the file", ex);
            }
        }



        #endregion
    }
}
