using MediatR;
using PetCare.API.Data;
using PetCare.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetCare.API.Features.HealthEvents
{
    public class CompleteOccurrenceResult
    {
        public bool Success { get; set; }
        public bool WasLastOccurrence { get; set; }
    }

    public class CompleteOccurrenceCommand : IRequest<CompleteOccurrenceResult>
    {
        public Guid UserId { get; set; }
        public Guid OccurrenceId { get; set; }
    }

    public class CompleteOccurrenceCommandHandler : IRequestHandler<CompleteOccurrenceCommand, CompleteOccurrenceResult>
    {
        private readonly AppDbContext _context;
        public CompleteOccurrenceCommandHandler(AppDbContext context) => _context = context;

        public async Task<CompleteOccurrenceResult> Handle(CompleteOccurrenceCommand request, CancellationToken cancellationToken)
        {
            var occurrence = await _context.HealthEventOccurrences
                .Include(o => o.HealthEvent)
                .ThenInclude(e => e.Pet)
                .FirstOrDefaultAsync(o => o.Id == request.OccurrenceId, cancellationToken);

            if (occurrence == null || occurrence.HealthEvent.Pet.UserId != request.UserId)
                return new CompleteOccurrenceResult { Success = false };

            occurrence.Status = OccurrenceStatus.Completed;
            occurrence.CompletedAt = DateTime.UtcNow;
            occurrence.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // Check if this was the last occurrence and the event has no EndDate
            bool wasLastOccurrence = false;
            if (occurrence.HealthEvent.EndDate == null)
            {
                int remainingPending = await _context.HealthEventOccurrences
                    .CountAsync(o => o.HealthEventId == occurrence.HealthEventId && o.Status == OccurrenceStatus.Pending, cancellationToken);
                
                if (remainingPending == 0)
                {
                    wasLastOccurrence = true;
                }
            }

            return new CompleteOccurrenceResult { Success = true, WasLastOccurrence = wasLastOccurrence };
        }
    }
}