using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.DTOs;

namespace PetCare.API.Features.Pets
{
    public class GetPetByIdQuery : IRequest<PetDto?>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetPetByIdHandler : IRequestHandler<GetPetByIdQuery, PetDto?>
    {
        private readonly AppDbContext _context;

        public GetPetByIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PetDto?> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == request.UserId, cancellationToken);

            if (pet == null) return null;

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
