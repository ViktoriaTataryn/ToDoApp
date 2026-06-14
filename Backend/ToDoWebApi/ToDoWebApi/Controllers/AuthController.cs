using Microsoft.AspNetCore.Mvc;
using ToDoApp.Core.DTOs;
using ToDoApp.Core.Interfaces;

namespace ToDoWebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterAsync([FromBody] RegisterUserDto registerUserDTO)
        {
            return Ok(await _authService.RegisterAsync(registerUserDTO));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> LoginAsync([FromBody]LoginDto loginDto, CancellationToken cancellationToken=default)
        {
            var result = await _authService.LoginAsync(loginDto, cancellationToken);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
