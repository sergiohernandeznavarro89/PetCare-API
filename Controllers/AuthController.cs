using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.DTOs;
using PetCare.API.Features.Auth;

namespace PetCare.API.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar la autenticación de usuarios.
    /// Contiene los endpoints para el registro y el inicio de sesión.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="dto">Los datos de registro (email, password).</param>
        /// <returns>La información del usuario creado.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            try
            {
                var response = await _mediator.Send(new RegisterUserCommand { Dto = dto });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Inicia sesión en el sistema validando credenciales y obteniendo un token JWT.
        /// </summary>
        /// <param name="dto">Credenciales de inicio de sesión.</param>
        /// <returns>Token JWT de acceso.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            try
            {
                var response = await _mediator.Send(new LoginUserCommand { Dto = dto });

                if (response == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
