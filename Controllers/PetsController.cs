using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;
using System.Security.Claims;

namespace PetCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetsController(AppDbContext context)
        {
            _context = context;
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

            var pets = await _context.Pets
                .Where(p => p.UserId == userId)
                .Select(p => new PetDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Species = p.Species,
                    Breed = p.Breed,
                    DateOfBirth = p.DateOfBirth,
                    PhotoUrl = p.PhotoUrl,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(pets);
        }

        // GET: api/Pets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PetDto>> GetPet(Guid id)
        {
            var userId = GetCurrentUserId();

            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (pet == null)
            {
                return NotFound();
            }

            return Ok(new PetDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                DateOfBirth = pet.DateOfBirth,
                PhotoUrl = pet.PhotoUrl,
                CreatedAt = pet.CreatedAt
            });
        }

        // POST: api/Pets
        [HttpPost]
        public async Task<ActionResult<PetDto>> PostPet(CreatePetDto dto)
        {
            var userId = GetCurrentUserId();

            var pet = new Pet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                DateOfBirth = dto.DateOfBirth?.ToUniversalTime(),
                PhotoUrl = dto.PhotoUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            var petDto = new PetDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                DateOfBirth = pet.DateOfBirth,
                PhotoUrl = pet.PhotoUrl,
                CreatedAt = pet.CreatedAt
            };

            return CreatedAtAction(nameof(GetPet), new { id = pet.Id }, petDto);
        }

        // PUT: api/Pets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(Guid id, CreatePetDto dto)
        {
            var userId = GetCurrentUserId();

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (pet == null)
            {
                return NotFound();
            }

            pet.Name = dto.Name;
            pet.Species = dto.Species;
            pet.Breed = dto.Breed;
            pet.DateOfBirth = dto.DateOfBirth?.ToUniversalTime();
            pet.PhotoUrl = dto.PhotoUrl;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Pets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(Guid id)
        {
            var userId = GetCurrentUserId();

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (pet == null)
            {
                return NotFound();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
