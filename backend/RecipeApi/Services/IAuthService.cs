using RecipeApi.DTOs;
using RecipeApi.DTOs.Auth;

namespace RecipeApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        string GenerateJwtToken(UserDto user);
    }
}