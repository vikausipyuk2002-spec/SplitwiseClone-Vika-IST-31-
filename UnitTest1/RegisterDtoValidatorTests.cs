using SplitwiseClone.DTOs;
using SplitwiseClone.Validators;
using Xunit;

namespace UnitTest1;

public class RegisterDtoValidatorTests
{
    private readonly RegisterDtoValidator _validator;

    public RegisterDtoValidatorTests()
    {
        _validator = new RegisterDtoValidator();
    }

    [Fact]
    public void Validate_ValidDto_ReturnsTrue()
    {
        var dto = new RegisterDto { Email = "valid@test.com", Password = "StrongPassword123!" };
        var result = _validator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyEmail_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "", Password = "StrongPassword123!" };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_NullEmail_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = null, Password = "StrongPassword123!" };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidEmailFormat_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "not-an-email", Password = "StrongPassword123!" };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyPassword_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = "" };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_NullPassword_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = null };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_PasswordTooShort_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = "Short1!" }; // 7 symbols
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_PasswordWithoutUppercase_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = "weakpassword123!" };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_PasswordWithoutLowercase_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = "WEAKPASSWORD123!" };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_PasswordWithoutDigit_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "test@test.com", Password = "StrongPassword!" };
        var result = _validator.Validate(dto);
        Assert.False(result.IsValid);
    }
}