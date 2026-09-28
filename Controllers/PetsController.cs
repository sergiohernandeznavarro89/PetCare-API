using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.DTOs;
using PetCare.API.Features.Pets;
using System.Security.Claims;

namespace PetCare.API.Controllers
{
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

        // GET: api/Pets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDto>>> GetPets()
        {
            var userId = GetCurrentUserId();
            var pets = await _mediator.Send(new GetPetsQuery { UserId = userId });
            return Ok(pets);
        }

        // GET: api/Pets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PetDto>> GetPet(Guid id)
        {
            var userId = GetCurrentUserId();
            var pet = await _mediator.Send(new GetPetByIdQuery { Id = id, UserId = userId });

            if (pet == null) return NotFound();
            
            return Ok(pet);
        }

        // POST: api/Pets
        [HttpPost]
        public async Task<ActionResult<PetDto>> PostPet(CreatePetDto dto)
        {
            var userId = GetCurrentUserId();
            var pet = await _mediator.Send(new CreatePetCommand { Dto = dto, UserId = userId });

            return CreatedAtAction(nameof(GetPet), new { id = pet.Id }, pet);
        }

        // PUT: api/Pets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(Guid id, CreatePetDto dto)
        {
            var userId = GetCurrentUserId();
            var success = await _mediator.Send(new UpdatePetCommand { Id = id, Dto = dto, UserId = userId });

            if (!success) return NotFound();

            return NoContent();
        }

        // DELETE: api/Pets/{id}
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
