using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.API.Features.HealthEvents
{
    public class GetHealthHistoryQuery : IRequest<IEnumerable<HealthEventDto>>
    {
        public Guid UserId { get; set; }
        public Guid PetId { get; set; }
    }

    public class GetHealthHistoryQueryHandler : IRequestHandler<GetHealthHistoryQuery, IEnumerable<HealthEventDto>>
    {
        private readonly AppDbContext _context;
        public GetHealthHistoryQueryHandler(AppDbContext context) => _context = context;

        public async Task<IEnumerable<HealthEventDto>> Handle(GetHealthHistoryQuery request, CancellationToken cancellationToken)
        {
            var events = await _context.HealthEvents
                .Include(e => e.EventType)
                .Include(e => e.Pet)
                .Where(e => e.PetId == request.PetId && e.Pet.UserId == request.UserId)
                .OrderByDescending(e => e.Date)
                .ToListAsync(cancellationToken);

            return events.Select(MapToDto);
        }

        private HealthEventDto MapToDto(HealthEvent e)
        {
            var eventTypeDto = new EventTypeDefinitionDto
            {
                Id = e.EventType.Id,
                UserId = e.EventType.UserId,
                Name = e.EventType.Name,
                Icon = e.EventType.Icon,
                Color = e.EventType.Color
            };

            return e switch
            {
                VetVisitEvent v => new VetVisitEventDto { Id = v.Id, PetId = v.PetId, Date = v.Date, Title = v.Title, Notes = v.Notes, Weight = v.Weight, EventType = eventTypeDto, VeterinarianName = v.VeterinarianName, ClinicName = v.ClinicName, Diagnosis = v.Diagnosis, IsHospitalization = v.IsHospitalization, DischargeDate = v.DischargeDate },
                MedicationEvent m => new MedicationEventDto { Id = m.Id, PetId = m.PetId, Date = m.Date, Title = m.Title, Notes = m.Notes, Weight = m.Weight, EventType = eventTypeDto, DrugName = m.DrugName, Dosage = m.Dosage, FrequencyValue = m.FrequencyValue, FrequencyUnit = (int)m.FrequencyUnit, StartDate = m.StartDate, EndDate = m.EndDate },
                VaccineEvent v => new VaccineEventDto { Id = v.Id, PetId = v.PetId, Date = v.Date, Title = v.Title, Notes = v.Notes, Weight = v.Weight, EventType = eventTypeDto, VaccineName = v.VaccineName, FrequencyValue = v.FrequencyValue, FrequencyUnit = (int)v.FrequencyUnit },
                CustomHealthEvent c => new CustomHealthEventDto { Id = c.Id, PetId = c.PetId, Date = c.Date, Title = c.Title, Notes = c.Notes, Weight = c.Weight, EventType = eventTypeDto, FrequencyValue = c.FrequencyValue, FrequencyUnit = (int)c.FrequencyUnit },
                _ => throw new NotImplementedException()
            };
        }
    }
}
