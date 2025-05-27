using Microsoft.AspNetCore.Mvc;
using RecipeApi.DTOs.Auth;
using RecipeApi.Services;

namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid username or password" });
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest(new { message = "Username or email already exists" });
            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For JWT, logout is handled client-side by deleting the token.
            return Ok(new { message = "Logged out" });
        }
    }
} 