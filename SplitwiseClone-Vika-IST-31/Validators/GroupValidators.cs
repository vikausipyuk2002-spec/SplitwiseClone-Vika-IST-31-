using FluentValidation;
using SplitwiseClone.DTOs;

namespace SplitwiseClone.Validators;

public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
{
    private static readonly string[] ReservedGroupNames = ["Admin", "System", "Root"];

    public CreateGroupDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва групи обов'язкова.")
            .Length(3, 100).WithMessage("Назва групи має бути від 3 до 100 символів.")
            .Must(n => !ReservedGroupNames.Contains(n, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Group name cannot be a reserved system name.");
    }
}

public class AddMemberDtoValidator : AbstractValidator<AddMemberDto>
{
    public AddMemberDtoValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseDtoValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Splits).NotEmpty().WithMessage("Має бути хоча б один учасник.");

        
        RuleFor(x => x)
            .Must(x => x.Splits.Sum(s => s.PaidShare) == x.Amount)
            .WithMessage("Сума PaidShare має дорівнювати загальній сумі витрати.");

        RuleFor(x => x)
            .Must(x => x.Splits.Sum(s => s.OwedShare) == x.Amount)
            .WithMessage("Сума OwedShare має дорівнювати загальній сумі витрати.");
    }
}

public class CreateSettlementDtoValidator : AbstractValidator<CreateSettlementDto>
{
    public CreateSettlementDtoValidator()
    {
        RuleFor(x => x.ToUserId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}