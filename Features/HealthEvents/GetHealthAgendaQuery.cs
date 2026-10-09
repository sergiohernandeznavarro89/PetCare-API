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
    public class GetHealthAgendaQuery : IRequest<IEnumerable<HealthEventOccurrenceDto>>
    {
        public Guid UserId { get; set; }
        public Guid? PetId { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
    }

    public class GetHealthAgendaQueryHandler : IRequestHandler<GetHealthAgendaQuery, IEnumerable<HealthEventOccurrenceDto>>
    {
        private readonly AppDbContext _context;
        public GetHealthAgendaQueryHandler(AppDbContext context) => _context = context;

        public async Task<IEnumerable<HealthEventOccurrenceDto>> Handle(GetHealthAgendaQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextMonthEnd = currentMonthStart.AddMonths(2).AddDays(-1);

            var query = _context.HealthEventOccurrences
                .Include(o => o.HealthEvent)
                .ThenInclude(e => e.EventType)
                .Include(o => o.HealthEvent)
                .ThenInclude(e => e.Pet)
                .Where(o => o.HealthEvent.Pet.UserId == request.UserId && 
                            o.ScheduledDate >= currentMonthStart && o.ScheduledDate <= nextMonthEnd &&
                            o.Status != OccurrenceStatus.Completed && o.Status != OccurrenceStatus.Cancelled); // No mostramos completados ni cancelados en la agenda activa

            if (request.PetId.HasValue && request.PetId.Value != Guid.Empty)
            {
                query = query.Where(o => o.HealthEvent.PetId == request.PetId.Value);
            }

            var occurrences = await query.OrderBy(o => o.ScheduledDate)
                                         .Skip(request.Skip)
                                         .Take(request.Take)
                                         .ToListAsync(cancellationToken);

            return occurrences.Select(o => new HealthEventOccurrenceDto
            {
                Id = o.Id,
                HealthEventId = o.HealthEventId,
                ScheduledDate = DateTime.SpecifyKind(o.ScheduledDate, DateTimeKind.Utc),
                Status = o.Status.ToString(),
                CompletedAt = o.CompletedAt,
                HealthEvent = MapToDto(o.HealthEvent)
            });
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