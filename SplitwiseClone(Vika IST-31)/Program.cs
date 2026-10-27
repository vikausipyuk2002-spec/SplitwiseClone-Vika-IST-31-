using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SplitwiseClone.Data;
using SplitwiseClone.Repositories;
using SplitwiseClone.Services;

var services = new ServiceCollection();
services.AddLogging();
services.AddDbContext<DB>();
services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<DB>();


services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IExpenseService, ExpenseService>();
services.AddScoped<IAuthService, AuthService>();

var serviceProvider = services.BuildServiceProvider();
var authService = serviceProvider.GetRequiredService<IAuthService>();

Console.WriteLine("Пробуємо зареєструвати користувача...");
var regResult = await authService.RegisterAsync("student@test.com", "StrongPass123!");

if (regResult.Succeeded)
{
    Console.WriteLine("Користувача успішно зареєстровано!");
}
else
{
    Console.WriteLine("Помилка реєстрації: " + string.Join(", ", regResult.Errors.Select(e => e.Description)));
}

Console.WriteLine("Перевіряємо пароль...");
bool isLogged = await authService.LoginAsync("student@test.com", "StrongPass123!");
Console.WriteLine(isLogged ? "Успішний вхід!" : "Невірний логін або пароль.");

Console.WriteLine("Шар бізнес-логіки (BLL) успішно підключено!");