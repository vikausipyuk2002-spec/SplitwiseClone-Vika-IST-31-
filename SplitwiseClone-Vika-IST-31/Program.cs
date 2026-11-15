using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using SplitwiseClone.Data;
using SplitwiseClone.DTOs;
using SplitwiseClone.Exceptions;
using SplitwiseClone.Repositories;
using SplitwiseClone.Services;
using SplitwiseClone.Validators;

var builder = WebApplication.CreateBuilder(args);

// Отримуємо налаштування з appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Підключення сервісів
builder.Services.AddDbContext<DB>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<DB>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// Підключаємо контролери (GroupsController, HealthController)
builder.Services.AddControllers();

// Додаємо Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Вмикаємо Swagger
app.UseSwagger();
app.UseSwaggerUI();


app.MapControllers();


app.MapPost("/api/auth/register", async (RegisterDto dto, IAuthService authService) =>
{
    try
    {
        var result = await authService.RegisterAsync(dto);
        if (result.Succeeded) return Results.Ok("Користувача успішно зареєстровано!");
        return Results.BadRequest(result.Errors.Select(e => e.Description));
    }
    catch (CustomValidationException ex)
    {
        return Results.BadRequest(ex.Errors);
    }
});

app.MapPost("/api/auth/login", async (string email, string password, IAuthService authService) =>
{
    bool isLogged = await authService.LoginAsync(email, password);
    return isLogged ? Results.Ok("Успішний вхід!") : Results.Unauthorized();
});


app.MapPost("/api/auth/refresh", () => Results.Ok("Token refreshed"));
app.MapGet("/api/auth/confirmEmail", () => Results.Ok("Email confirmed"));
app.MapPost("/api/auth/resendConfirmationEmail", () => Results.Ok("Confirmation email resent"));
app.MapPost("/api/auth/forgotPassword", () => Results.Ok("Password reset link sent"));
app.MapPost("/api/auth/resetPassword", () => Results.Ok("Password reset"));
app.MapPost("/api/auth/manage/2fa", () => Results.Ok("2FA managed"));
app.MapGet("/api/auth/manage/info", () => Results.Ok("Manage info retrieved"));
app.MapPost("/api/auth/manage/info", () => Results.Ok("Manage info updated"));

app.Run();