using authService.Domain.Interfaces;
using AuthService.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace authService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthJwtService _authService;

        public AuthController(IAuthJwtService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }

        [HttpPut("change_password")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdateDto updateDto)
        {
            await _authService.ChangePasswordAsync(updateDto);
            return Ok();
        }
        
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteDto deleteUserDto)
        {
            await _authService.DeleteAsync(deleteUserDto);
            return Ok();
        }
    }
}
