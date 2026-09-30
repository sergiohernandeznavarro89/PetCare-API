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
    public class CreateHealthEventCommand : IRequest<HealthEventDto>
    {
        public Guid UserId { get; set; }
        public HealthEventDto EventDto { get; set; } = null!;
    }

    public class CreateHealthEventCommandHandler : IRequestHandler<CreateHealthEventCommand, HealthEventDto>
    {
        private readonly AppDbContext _context;
        public CreateHealthEventCommandHandler(AppDbContext context) => _context = context;

        public async Task<HealthEventDto> Handle(CreateHealthEventCommand request, CancellationToken cancellationToken)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == request.EventDto.PetId && p.UserId == request.UserId, cancellationToken);
            if (pet == null) throw new UnauthorizedAccessException();

            var eventType = await _context.EventTypeDefinitions.FindAsync(request.EventDto.EventType.Id);
            if (eventType == null) throw new ArgumentException("EventType no encontrado");

            HealthEvent entity = request.EventDto switch
            {
                VetVisitEventDto v => new VetVisitEvent { PetId = v.PetId, EventTypeId = eventType.Id, Date = v.Date.ToUniversalTime(), Title = v.Title, Notes = v.Notes, Weight = v.Weight, VeterinarianName = v.VeterinarianName, ClinicName = v.ClinicName, Diagnosis = v.Diagnosis, IsHospitalization = v.IsHospitalization, DischargeDate = v.DischargeDate?.ToUniversalTime() },
                MedicationEventDto m => new MedicationEvent { PetId = m.PetId, EventTypeId = eventType.Id, Date = m.Date.ToUniversalTime(), Title = m.Title, Notes = m.Notes, Weight = m.Weight, DrugName = m.DrugName, Dosage = m.Dosage, FrequencyValue = m.FrequencyValue, FrequencyUnit = (FrequencyUnit)m.FrequencyUnit, StartDate = m.StartDate.ToUniversalTime(), EndDate = m.EndDate?.ToUniversalTime() },
                VaccineEventDto v => new VaccineEvent { PetId = v.PetId, EventTypeId = eventType.Id, Date = v.Date.ToUniversalTime(), Title = v.Title, Notes = v.Notes, Weight = v.Weight, VaccineName = v.VaccineName, FrequencyValue = v.FrequencyValue, FrequencyUnit = (FrequencyUnit)v.FrequencyUnit },
                CustomHealthEventDto c => new CustomHealthEvent { PetId = c.PetId, EventTypeId = eventType.Id, Date = c.Date.ToUniversalTime(), Title = c.Title, Notes = c.Notes, Weight = c.Weight, FrequencyValue = c.FrequencyValue, FrequencyUnit = (FrequencyUnit)c.FrequencyUnit },
                _ => throw new NotImplementedException()
            };

            _context.HealthEvents.Add(entity);

            int freqVal = 0;
            FrequencyUnit freqUnit = FrequencyUnit.Days;
            
            if (entity is MedicationEvent medEv) { freqVal = medEv.FrequencyValue; freqUnit = medEv.FrequencyUnit; }
            else if (entity is VaccineEvent vacEv) { freqVal = vacEv.FrequencyValue; freqUnit = vacEv.FrequencyUnit; }
            else if (entity is CustomHealthEvent cusEv) { freqVal = cusEv.FrequencyValue; freqUnit = cusEv.FrequencyUnit; }

            var occurrences = HealthEventOccurrenceGenerator.GenerateOccurrences(entity, freqVal, freqUnit);
            _context.HealthEventOccurrences.AddRange(occurrences);

            await _context.SaveChangesAsync(cancellationToken);

            // Re-map to return
            request.EventDto.Id = entity.Id;
            return request.EventDto;
        }
    }
}
