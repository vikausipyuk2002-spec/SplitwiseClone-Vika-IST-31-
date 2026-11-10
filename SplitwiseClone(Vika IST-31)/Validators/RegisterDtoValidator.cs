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
            .NotEmpty().WithMessage("Пароль не може бути порожнім.")
            .MinimumLength(8).WithMessage("Пароль має містити мінімум 8 символів.")
            .Matches("[A-Z]").WithMessage("Пароль має містити хоча б одну велику літеру.")
            .Matches("[a-z]").WithMessage("Пароль має містити хоча б одну малу літеру.")
            .Matches("[0-9]").WithMessage("Пароль має містити хоча б одну цифру.");
    }
}