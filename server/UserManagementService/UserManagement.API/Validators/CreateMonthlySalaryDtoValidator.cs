using FluentValidation;
using UserManagement.API.Dtos.WorkShift;

namespace UserManagement.API.Validators
{
    public class CreateMonthlySalaryDtoValidator : AbstractValidator<CreateMonthlySalaryDto>
    {
        public CreateMonthlySalaryDtoValidator()
        {
            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");

            RuleFor(x => x.Year)
                .GreaterThanOrEqualTo(2000).WithMessage("Year must be greater than or equal to 2000.");

            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("EmployeeId is required.");

            RuleFor(x => x.TotalHour)
                .GreaterThanOrEqualTo(0).WithMessage("TotalHour must be a non-negative value.");

            RuleFor(x => x.Bonus)
                .GreaterThanOrEqualTo(0).WithMessage("Bonus must be a non-negative value.");

            RuleFor(x => x.Penalty)
                .GreaterThanOrEqualTo(0).WithMessage("Penalty must be a non-negative value.");

            RuleFor(x => x.NetSalary)
                .GreaterThanOrEqualTo(0).WithMessage("NetSalary must be a non-negative value.");

            RuleFor(x => x.TotalNumberOfShifts)
                .GreaterThanOrEqualTo(0).WithMessage("TotalNumberOfShifts must be a non-negative integer.");

            RuleFor(x => x.TotalLateShifts)
                .GreaterThanOrEqualTo(0).WithMessage("TotalLateShifts must be a non-negative integer.");

            RuleFor(x => x.TotalAbsentShifts)
                .GreaterThanOrEqualTo(0).WithMessage("TotalAbsentShifts must be a non-negative integer.");

            RuleFor(x => x.TotalNotCompleteShifts)
                .GreaterThanOrEqualTo(0).WithMessage("TotalNotCompleteShifts must be a non-negative integer.");

            RuleFor(x => x.SalaryBase)
                .GreaterThanOrEqualTo(0).WithMessage("SalaryBase must be a non-negative value.");

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("BranchId is required.");
        }
    }
}
