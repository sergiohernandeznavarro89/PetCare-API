using MediatR;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetCare.API.Features.HealthEvents
{
    public class UpdateHealthEventCommand : IRequest<HealthEventDto?>
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public HealthEventDto EventDto { get; set; } = null!;
    }

    public class UpdateHealthEventCommandHandler : IRequestHandler<UpdateHealthEventCommand, HealthEventDto?>
    {
        private readonly AppDbContext _context;
        public UpdateHealthEventCommandHandler(AppDbContext context) => _context = context;

        public async Task<HealthEventDto?> Handle(UpdateHealthEventCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.HealthEvents
                .Include(e => e.Pet)
                .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

            if (entity == null || entity.Pet.UserId != request.UserId) return null;

            var eventType = await _context.EventTypeDefinitions.FindAsync(new object[] { request.EventDto.EventType.Id }, cancellationToken);
            if (eventType == null) throw new ArgumentException("EventType no encontrado");

            entity.EventTypeId = eventType.Id;
            entity.Date = request.EventDto.Date.ToUniversalTime();
            entity.Title = request.EventDto.Title;
            entity.Notes = request.EventDto.Notes;
            entity.Weight = request.EventDto.Weight;
            entity.ParentVisitId = string.IsNullOrEmpty(request.EventDto.ParentId) ? null : Guid.Parse(request.EventDto.ParentId);
            entity.EndDate = request.EventDto.EndDate?.ToUniversalTime();
            entity.UpdatedAt = DateTime.UtcNow;

            switch (request.EventDto)
            {
                case VetVisitEventDto v when entity is VetVisitEvent vet:
                    vet.VeterinarianName = v.VeterinarianName;
                    vet.ClinicName = v.ClinicName;
                    vet.Diagnosis = v.Diagnosis;
                    vet.IsHospitalization = v.IsHospitalization;
                    vet.DischargeDate = v.DischargeDate?.ToUniversalTime();
                    break;
                case MedicationEventDto m when entity is MedicationEvent med:
                    med.DrugName = m.DrugName;
                    med.Dosage = m.Dosage;
                    med.FrequencyValue = m.FrequencyValue;
                    med.FrequencyUnit = (FrequencyUnit)m.FrequencyUnit;
                    med.StartDate = m.StartDate.ToUniversalTime();
                    break;
                case VaccineEventDto v when entity is VaccineEvent vac:
                    vac.VaccineName = v.VaccineName;
                    vac.FrequencyValue = v.FrequencyValue;
                    vac.FrequencyUnit = (FrequencyUnit)v.FrequencyUnit;
                    break;
                case CustomHealthEventDto c when entity is CustomHealthEvent custom:
                    custom.FrequencyValue = c.FrequencyValue;
                    custom.FrequencyUnit = (FrequencyUnit)c.FrequencyUnit;
                    break;
                default:
                    throw new InvalidOperationException("No se puede cambiar el tipo de evento base o los tipos no coinciden.");
            }

            var pendingOccurrences = await _context.HealthEventOccurrences
                .Where(o => o.HealthEventId == entity.Id && o.Status == OccurrenceStatus.Pending)
                .ToListAsync(cancellationToken);
            
            _context.HealthEventOccurrences.RemoveRange(pendingOccurrences);

            var existingCompleted = await _context.HealthEventOccurrences
                .Where(o => o.HealthEventId == entity.Id && o.Status != OccurrenceStatus.Pending)
                .ToListAsync(cancellationToken);

            int freqVal = 0;
            FrequencyUnit freqUnit = FrequencyUnit.Days;
            
            if (entity is MedicationEvent medEv) { freqVal = medEv.FrequencyValue; freqUnit = medEv.FrequencyUnit; }
            else if (entity is VaccineEvent vacEv) { freqVal = vacEv.FrequencyValue; freqUnit = vacEv.FrequencyUnit; }
            else if (entity is CustomHealthEvent cusEv) { freqVal = cusEv.FrequencyValue; freqUnit = cusEv.FrequencyUnit; }

            var newOccurrences = HealthEventOccurrenceGenerator.GenerateOccurrences(entity, freqVal, freqUnit);
            
            var completedDates = existingCompleted.Select(c => c.ScheduledDate).ToHashSet();
            var occurrencesToAdd = newOccurrences.Where(o => !completedDates.Contains(o.ScheduledDate)).ToList();

            _context.HealthEventOccurrences.AddRange(occurrencesToAdd);

            await _context.SaveChangesAsync(cancellationToken);

            request.EventDto.Id = entity.Id;
            return request.EventDto;
        }
    }
}