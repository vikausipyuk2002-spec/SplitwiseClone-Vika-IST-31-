using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SplitwiseClone.DTOs;
using SplitwiseClone.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace SplitwiseClone.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IValidator<RegisterDto> _validator;

   
    public AuthService(UserManager<IdentityUser> userManager, IValidator<RegisterDto> validator)
    {
        _userManager = userManager;
        _validator = validator;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        // 1. Перевірка вхідних даних через FluentValidation
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            throw new CustomValidationException(errors);
        }

        // 2. Якщо валідація успішна, створюємо користувача
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
        return await _userManager.CreateAsync(user, dto.Password);
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        return false;
    }
}