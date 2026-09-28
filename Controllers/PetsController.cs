using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.DTOs;
using PetCare.API.Features.Pets;
using System.Security.Claims;

namespace PetCare.API.Controllers
{
    /// <summary>
    /// Controlador responsable de gestionar las mascotas (CRUD).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in token.");
        }

        /// <summary>
        /// Obtiene todas las mascotas del usuario autenticado.
        /// </summary>
        /// <returns>Una colección de mascotas.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDto>>> GetPets()
        {
            var userId = GetCurrentUserId();
            var pets = await _mediator.Send(new GetPetsQuery { UserId = userId });
            return Ok(pets);
        }

        /// <summary>
        /// Obtiene una mascota concreta por su identificador.
        /// </summary>
        /// <param name="id">El ID único de la mascota.</param>
        /// <returns>La información de la mascota solicitada.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PetDto>> GetPet(Guid id)
        {
            var userId = GetCurrentUserId();
            var pet = await _mediator.Send(new GetPetByIdQuery { Id = id, UserId = userId });

            if (pet == null) return NotFound();
            
            return Ok(pet);
        }

        /// <summary>
        /// Crea una nueva mascota y la asocia al usuario actual.
        /// </summary>
        /// <param name="dto">Los datos de la mascota a crear.</param>
        /// <returns>La mascota recién creada.</returns>
        [HttpPost]
        public async Task<ActionResult<PetDto>> PostPet(CreatePetDto dto)
        {
            var userId = GetCurrentUserId();
            var pet = await _mediator.Send(new CreatePetCommand { Dto = dto, UserId = userId });

            return CreatedAtAction(nameof(GetPet), new { id = pet.Id }, pet);
        }

        /// <summary>
        /// Actualiza la información de una mascota existente.
        /// </summary>
        /// <param name="id">El ID de la mascota a modificar.</param>
        /// <param name="dto">Los nuevos datos.</param>
        /// <returns>Respuesta HTTP NoContent o NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(Guid id, CreatePetDto dto)
        {
            var userId = GetCurrentUserId();
            var success = await _mediator.Send(new UpdatePetCommand { Id = id, Dto = dto, UserId = userId });

            if (!success) return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Elimina permanentemente a una mascota.
        /// </summary>
        /// <param name="id">El ID de la mascota a eliminar.</param>
        /// <returns>Respuesta HTTP NoContent o NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(Guid id)
        {
            var userId = GetCurrentUserId();
            var success = await _mediator.Send(new DeletePetCommand { Id = id, UserId = userId });

            if (!success) return NotFound();

            return NoContent();
        }
    }
}
