using KoopaBackend.Application.Services;
using KoopaBackend.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace KoopaBackend.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _service;

        public AuthController(UsuarioService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.NombreUsuario) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new LoginResponseDto
                {
                    Success = false,
                    Message = "Nombre de usuario y contraseña son requeridos"
                });
            }

            var result = await _service.LoginAsync(request);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestDto request)
        {
            if (string.IsNullOrEmpty(request.NombreUsuario) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new LoginResponseDto
                {
                    Success = false,
                    Message = "Nombre de usuario y contraseña son requeridos"
                });
            }

            var result = await _service.CreateUserAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

