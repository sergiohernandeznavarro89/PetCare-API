using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;

namespace PetCare.API.Features.Pets
{
    public class DeletePetCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeletePetHandler : IRequestHandler<DeletePetCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeletePetHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeletePetCommand request, CancellationToken cancellationToken)
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == request.UserId, cancellationToken);

            if (pet == null) return false;

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
