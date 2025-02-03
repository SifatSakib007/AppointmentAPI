using AppointmentAPI.Models;
using AppointmentAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns the validation error messages
            }
            var result = _authService.RegisterUser(request.Username, request.Password);
            if(result == "User already exists")
            {
                return BadRequest(new {message = result});
            }
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto request)
        {
            var token = _authService.AuthenticateUser(request.Username, request.Password);
            if (token == null)
            {
                return Unauthorized(new {message = "Invalid credentials"});
            }
            return Ok(new { token, message = "JWT Token generated successfully" });
        }
    }
}
