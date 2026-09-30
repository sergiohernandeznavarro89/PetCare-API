using MediatR;
using PetCare.API.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetCare.API.Features.HealthEvents
{
    public class DeleteHealthEventCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class DeleteHealthEventCommandHandler : IRequestHandler<DeleteHealthEventCommand, bool>
    {
        private readonly AppDbContext _context;
        public DeleteHealthEventCommandHandler(AppDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteHealthEventCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.HealthEvents
                .Include(e => e.Pet)
                .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

            if (entity == null || entity.Pet.UserId != request.UserId) return false;

            _context.HealthEvents.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}