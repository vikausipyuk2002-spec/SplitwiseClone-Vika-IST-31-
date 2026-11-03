using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SplitwiseClone.Data;
using SplitwiseClone.DTOs;
using SplitwiseClone.Exceptions;
using SplitwiseClone.Repositories;
using SplitwiseClone.Services;
using SplitwiseClone.Validators;
using System.Text;
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var services = new ServiceCollection();
services.AddLogging();
services.AddDbContext<DB>();
services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<DB>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IExpenseService, ExpenseService>();
services.AddScoped<IAuthService, AuthService>();

// НОВЕ: Реєстрація валідаторів FluentValidation
services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

var serviceProvider = services.BuildServiceProvider();
var authService = serviceProvider.GetRequiredService<IAuthService>();

Console.WriteLine("Пробуємо зареєструвати користувача...");

// НОВЕ: Створила DTO об'єкт для передачі даних
var registerDto = new RegisterDto
{
    Email = "student2@test.com", // Змінила пошту, бо перший вже є в базі
    Password = "StrongPass123!"
};

// НОВЕ: Блок try-catch для перехоплення нашої CustomValidationException
try
{
    var regResult = await authService.RegisterAsync(registerDto); // Передаємо DTO замість двох рядків

    if (regResult.Succeeded)
    {
        Console.WriteLine("Користувача успішно зареєстровано!");
    }
    else
    {
        Console.WriteLine("Помилка реєстрації бази даних: " + string.Join(", ", regResult.Errors.Select(e => e.Description)));
    }
}
catch (CustomValidationException ex)
{
    // Якщо FluentValidation знайде помилки, програма зайде сюди
    Console.WriteLine("Помилка валідації DTO:");
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"- {error}");
    }
}

Console.WriteLine("Перевіряємо пароль...");
bool isLogged = await authService.LoginAsync("student2@test.com", "StrongPass123!");
Console.WriteLine(isLogged ? "Успішний вхід!" : "Невірний логін або пароль.");
Console.WriteLine("Шар бізнес-логіки (BLL) успішно підключено!");