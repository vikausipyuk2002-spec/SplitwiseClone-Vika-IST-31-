using FluentValidation;
using SplitwiseClone.DTOs;

namespace SplitwiseClone.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не може бути порожнім.")
            .EmailAddress().WithMessage("Невірний формат Email.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль є обов'язковим.")
            .MinimumLength(6).WithMessage("Пароль має містити мінімум 6 символів.");
    }
}