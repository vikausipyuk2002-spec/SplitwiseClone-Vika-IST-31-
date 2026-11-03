using Microsoft.AspNetCore.Identity;
using SplitwiseClone.DTOs;
using System.Threading.Tasks;

namespace SplitwiseClone.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterDto dto);
    Task<bool> LoginAsync(string email, string password);
}