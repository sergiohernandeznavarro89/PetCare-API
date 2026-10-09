using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.DTOs;

namespace PetCare.API.Features.Pets
{
    public class UpdatePetCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public CreatePetDto Dto { get; set; } = null!;
    }

    public class UpdatePetHandler : IRequestHandler<UpdatePetCommand, bool>
    {
        private readonly AppDbContext _context;

        public UpdatePetHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdatePetCommand request, CancellationToken cancellationToken)
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == request.UserId, cancellationToken);

            if (pet == null) return false;

            pet.Name = request.Dto.Name;
            pet.Species = request.Dto.Species;
            pet.Breed = request.Dto.Breed;
            pet.DateOfBirth = request.Dto.DateOfBirth?.ToUniversalTime();
            pet.PhotoUrl = request.Dto.PhotoUrl;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
