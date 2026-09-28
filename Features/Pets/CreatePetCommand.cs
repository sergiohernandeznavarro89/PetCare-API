using MediatR;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;

namespace PetCare.API.Features.Pets
{
    public class CreatePetCommand : IRequest<PetDto>
    {
        public CreatePetDto Dto { get; set; } = null!;
        public Guid UserId { get; set; }
    }

    public class CreatePetHandler : IRequestHandler<CreatePetCommand, PetDto>
    {
        private readonly AppDbContext _context;

        public CreatePetHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PetDto> Handle(CreatePetCommand request, CancellationToken cancellationToken)
        {
            var pet = new Pet
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Dto.Name,
                Species = request.Dto.Species,
                Breed = request.Dto.Breed,
                DateOfBirth = request.Dto.DateOfBirth?.ToUniversalTime(),
                PhotoUrl = request.Dto.PhotoUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync(cancellationToken);

            return new PetDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                DateOfBirth = pet.DateOfBirth,
                PhotoUrl = pet.PhotoUrl,
                CreatedAt = pet.CreatedAt
            };
        }
    }
}
