using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NovoHorizonteDigital.API.DTOs;
using NovoHorizonteDigital.API.Models;
using NovoHorizonteDigital.API.Services;

namespace NovoHorizonteDigital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (token, user) = await _authService.LoginAsync(request.Email, request.Password);
                
                var response = new AuthResponse
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role.ToString()
                    }
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!System.Enum.TryParse<UserRole>(request.Role, out var role))
                    return BadRequest(new { message = "Invalid role" });

                var user = await _authService.RegisterUserAsync(
                    request.Email, 
                    request.Password, 
                    request.FullName, 
                    request.PhoneNumber, 
                    role
                );

                var token = _authService.GenerateJwtToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role.ToString()
                    }
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
