using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using iTextSharp.text.pdf;
using iTextSharp.text;
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
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (MemoryStream stream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A5, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // Load font hỗ trợ tiếng Việt
                    string fontPath = Path.Combine(_env.WebRootPath, "fonts", "arialuni.ttf");

                    // Kiểm tra nếu file không tồn tại
                    if (!File.Exists(fontPath))
                    {
                        throw new FileNotFoundException($"Font file not found: {fontPath}");
                    }
                    BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font fontTitle = new Font(bf, 16, Font.BOLD);
                    Font fontHeader = new Font(bf, 12, Font.BOLD);
                    Font fontBody = new Font(bf, 12);
                    Font fontFooter = new Font(bf, 10, Font.ITALIC);

                    // Tiêu đề
                    Paragraph title = new Paragraph("PHIẾU LƯƠNG NHÂN VIÊN", fontTitle);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 10;
                    document.Add(title);

                    // Thông tin chung
                    Paragraph info = new Paragraph($"Kỳ lương: Tháng {salarySlip.Month}/{salarySlip.Year}", fontHeader);
                    info.SpacingAfter = 10;
                    document.Add(info);

                    // Tạo bảng
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 40f, 60f });

                    void AddHeaderCell(string text)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(text, fontHeader));
                        cell.BackgroundColor = new BaseColor(220, 220, 220);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }

                    void AddDataCell(string text)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(text, fontBody));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }

                    AddHeaderCell("Thông tin");
                    AddHeaderCell("Giá trị");

                    AddHeaderCell("Tên Nhân Viên");
                    AddDataCell(salarySlip.Employee.FullName);
                    AddHeaderCell("Lương Cơ Bản");
                    AddDataCell($"{salarySlip.SalaryBase:N0} VND");
                    AddHeaderCell("Số Giờ Làm");
                    AddDataCell($"{salarySlip.TotalHours}");
                    AddHeaderCell("Số Ca Làm");
                    AddDataCell($"{salarySlip.TotalNumberOfShifts}");
                    AddHeaderCell("Số Ca Đến Muộn");
                    AddDataCell($"{salarySlip.TotalLateShifts}");
                    AddHeaderCell("Số Ca Nghỉ");
                    AddDataCell($"{salarySlip.TotalAbsentShifts}");
                    AddHeaderCell("Số Ca Không Hoàn Thành");
                    AddDataCell($"{salarySlip.TotalNotCompleteShifts}");
                    AddHeaderCell("Thưởng");
                    AddDataCell($"{salarySlip.Bonus:N0} VND");
                    AddHeaderCell("Phạt");
                    AddDataCell($"{salarySlip.Penalty:N0} VND");
                    AddHeaderCell("Tổng Lương");
                    AddDataCell($"{salarySlip.NetSalary:N0} VND");

                    document.Add(table);
                    // Thêm ngày lập phiếu
                    Paragraph date = new Paragraph($"Ngày lập phiếu: {DateTime.Now:dd/MM/yyyy}", fontBody)
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        SpacingAfter = 10
                    };
                    document.Add(date);

                    // Thêm ghi chú hoặc lời nhắn
                    Paragraph note = new Paragraph("Cảm ơn bạn đã làm việc chăm chỉ! Chúc bạn một ngày tốt lành!", fontFooter)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    document.Add(note);

                    // Thêm tên quán vào góc dưới bên phải
                    string storeName = "Xóm Cafe"; // Đổi thành tên quán của bạn
                    ColumnText.ShowTextAligned(
                        writer.DirectContent,
                        Element.ALIGN_RIGHT,
                        new Phrase(storeName, fontFooter),
                        document.Right, // Cạnh phải
                        document.Bottom - 20, // Xuống sát mép dưới
                        0
                    );

                    document.Close();
                    return stream.ToArray();
                }
            }
            catch (FileNotFoundException fnfEx)
            {
                _logger.LogError("File error: {0}", fnfEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while creating file: {0}", ex);
                throw new Exception("An error occurred while creating the file", ex);
            }
        }



        private void AddCellToHeader(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(240, 240, 240);
            table.AddCell(cell);
        }

        private void AddCellToBody(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
        }


        #endregion
    }
}
