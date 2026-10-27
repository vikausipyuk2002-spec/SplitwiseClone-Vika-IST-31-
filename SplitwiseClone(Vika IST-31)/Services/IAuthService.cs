using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace SplitwiseClone.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(string email, string password);
    Task<bool> LoginAsync(string email, string password);
}