using MediatR;
using PetCare.API.Data;
using PetCare.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetCare.API.Features.HealthEvents
{
    public class CompleteOccurrenceCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid OccurrenceId { get; set; }
    }

    public class CompleteOccurrenceCommandHandler : IRequestHandler<CompleteOccurrenceCommand, bool>
    {
        private readonly AppDbContext _context;
        public CompleteOccurrenceCommandHandler(AppDbContext context) => _context = context;

        public async Task<bool> Handle(CompleteOccurrenceCommand request, CancellationToken cancellationToken)
        {
            var occurrence = await _context.HealthEventOccurrences
                .Include(o => o.HealthEvent)
                .ThenInclude(e => e.Pet)
                .FirstOrDefaultAsync(o => o.Id == request.OccurrenceId, cancellationToken);

            if (occurrence == null || occurrence.HealthEvent.Pet.UserId != request.UserId)
                return false;

            occurrence.Status = OccurrenceStatus.Completed;
            occurrence.CompletedAt = DateTime.UtcNow;
            occurrence.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}