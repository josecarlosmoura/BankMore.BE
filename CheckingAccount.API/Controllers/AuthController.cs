using Application.Exeption;
using Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticateUserService _authService;

        public AuthController(AuthenticateUserService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.AuthenticateAsync(request.cpf, request.numero, request.Password);
                return Ok(new { Token = token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                var error = new ApiError(ex.Error);
                return StatusCode(StatusCodes.Status400BadRequest, error);
            }
        }
    }

    public record LoginRequest(string cpf, long numero, string Password);
}
