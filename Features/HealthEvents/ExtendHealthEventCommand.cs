using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.API.Features.HealthEvents
{
    public class ExtendHealthEventCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class ExtendHealthEventCommandHandler : IRequestHandler<ExtendHealthEventCommand, bool>
    {
        private readonly AppDbContext _context;

        public ExtendHealthEventCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ExtendHealthEventCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.HealthEvents
                .Include(e => e.Pet)
                .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

            if (entity == null || entity.Pet.UserId != request.UserId)
                return false;

            // Only extend if there's no fixed EndDate
            if (entity.EndDate != null)
                return false;

            var existingOccurrences = await _context.HealthEventOccurrences
                .Where(o => o.HealthEventId == entity.Id)
                .ToListAsync(cancellationToken);

            // Find the last generated date
            var lastDate = existingOccurrences.Count > 0
                ? existingOccurrences.Max(o => o.ScheduledDate)
                : entity.Date;

            int freqVal = 0;
            FrequencyUnit freqUnit = FrequencyUnit.Days;
            
            if (entity is MedicationEvent medEv) { freqVal = medEv.FrequencyValue; freqUnit = medEv.FrequencyUnit; }
            else if (entity is VaccineEvent vacEv) { freqVal = vacEv.FrequencyValue; freqUnit = vacEv.FrequencyUnit; }
            else if (entity is CustomHealthEvent cusEv) { freqVal = cusEv.FrequencyValue; freqUnit = cusEv.FrequencyUnit; }

            // Temporary override to generate 2 more years from the last date
            var endDateOverride = lastDate.AddYears(2);

            var newOccurrences = HealthEventOccurrenceGenerator.GenerateOccurrences(entity, freqVal, freqUnit, endDateOverride);
            
            var existingDates = existingOccurrences.Select(c => c.ScheduledDate).ToHashSet();
            var occurrencesToAdd = newOccurrences.Where(o => !existingDates.Contains(o.ScheduledDate) && o.ScheduledDate > lastDate).ToList();

            if (occurrencesToAdd.Count > 0)
            {
                _context.HealthEventOccurrences.AddRange(occurrencesToAdd);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}
