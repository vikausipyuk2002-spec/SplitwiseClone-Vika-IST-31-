using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using SplitwiseClone.DTOs;
using SplitwiseClone.Exceptions;
using SplitwiseClone.Services;
using Xunit;

namespace UnitTest1;

public class AuthServiceTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IValidator<RegisterDto>> _validatorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var storeMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            storeMock.Object, null, null, null, null, null, null, null, null);

        _validatorMock = new Mock<IValidator<RegisterDto>>();
        _authService = new AuthService(_userManagerMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ValidDto_ReturnsSuccess()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = "StrongPassword123!" };
        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), dto.Password)).ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync(dto);

        Assert.True(result.Succeeded);
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), dto.Password), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_InvalidDto_ThrowsCustomValidationException()
    {
        var dto = new RegisterDto { Email = "", Password = "123" };
        var validationFailures = new List<ValidationFailure> { new ValidationFailure("Email", "Error") };
        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(validationFailures));

        await Assert.ThrowsAsync<CustomValidationException>(() => _authService.RegisterAsync(dto));
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_IdentityFails_ReturnsFailedResult()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = "weak" };
        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), dto.Password))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var result = await _authService.RegisterAsync(dto);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Description == "Password too weak");
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTrue()
    {
        var email = "test@test.com";
        var password = "StrongPassword123!";
        var user = new IdentityUser { Email = email, UserName = email };
        _userManagerMock.Setup(u => u.FindByEmailAsync(email)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, password)).ReturnsAsync(true);

        var result = await _authService.LoginAsync(email, password);

        Assert.True(result);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsFalse()
    {
        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
        var result = await _authService.LoginAsync("notfound@test.com", "Password123!");
        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsFalse()
    {
        var user = new IdentityUser { Email = "test@test.com", UserName = "test@test.com" };
        _userManagerMock.Setup(u => u.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "WrongPassword!")).ReturnsAsync(false);

        var result = await _authService.LoginAsync("test@test.com", "WrongPassword!");

        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_NullEmail_ReturnsFalse()
    {
        _userManagerMock.Setup(u => u.FindByEmailAsync(null)).ReturnsAsync((IdentityUser)null);
        var result = await _authService.LoginAsync(null, "Password123!");
        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_EmptyEmail_ReturnsFalse()
    {
        _userManagerMock.Setup(u => u.FindByEmailAsync("")).ReturnsAsync((IdentityUser)null);
        var result = await _authService.LoginAsync("", "Password123!");
        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_NullPassword_ReturnsFalse()
    {
        var user = new IdentityUser { Email = "test@test.com" };
        _userManagerMock.Setup(u => u.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, null)).ReturnsAsync(false);
        var result = await _authService.LoginAsync("test@test.com", null);
        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_EmptyPassword_ReturnsFalse()
    {
        var user = new IdentityUser { Email = "test@test.com" };
        _userManagerMock.Setup(u => u.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "")).ReturnsAsync(false);
        var result = await _authService.LoginAsync("test@test.com", "");
        Assert.False(result);
    }
}