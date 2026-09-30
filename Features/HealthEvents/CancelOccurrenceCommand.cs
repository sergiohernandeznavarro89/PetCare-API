using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.API.Features.HealthEvents
{
    public class CancelOccurrenceCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid OccurrenceId { get; set; }
    }

    public class CancelOccurrenceCommandHandler : IRequestHandler<CancelOccurrenceCommand, bool>
    {
        private readonly AppDbContext _context;

        public CancelOccurrenceCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CancelOccurrenceCommand request, CancellationToken cancellationToken)
        {
            var occurrence = await _context.HealthEventOccurrences
                .Include(o => o.HealthEvent)
                .ThenInclude(e => e.Pet)
                .FirstOrDefaultAsync(o => o.Id == request.OccurrenceId && o.HealthEvent.Pet.UserId == request.UserId, cancellationToken);

            if (occurrence == null)
            {
                return false;
            }

            occurrence.Status = OccurrenceStatus.Cancelled;
            occurrence.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
