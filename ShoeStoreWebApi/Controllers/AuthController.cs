using Microsoft.AspNetCore.Mvc;
using ShoeStoreLibrary.DTOs;
using ShoeStoreLibrary.Services;

namespace ShoeStoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Метод для входа
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null || string.IsNullOrWhiteSpace(userLoginDto.Login) || string.IsNullOrWhiteSpace(userLoginDto.Password))
            {
                return BadRequest("Invalid client request");
            }

            var token = _authService.Authenticate(userLoginDto.Login, userLoginDto.Password);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            return Ok(new { Token = token });
        }

    }
}