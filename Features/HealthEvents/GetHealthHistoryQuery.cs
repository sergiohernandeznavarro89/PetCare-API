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
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class GetHealthHistoryQueryHandler : IRequestHandler<GetHealthHistoryQuery, IEnumerable<HealthEventDto>>
    {
        private readonly AppDbContext _context;
        public GetHealthHistoryQueryHandler(AppDbContext context) => _context = context;

        public async Task<IEnumerable<HealthEventDto>> Handle(GetHealthHistoryQuery request, CancellationToken cancellationToken)
        {
            var parents = await _context.HealthEvents
                .Include(e => e.EventType)
                .Include(e => e.Pet)
                .Include(e => e.Occurrences)
                .Where(e => e.PetId == request.PetId && e.Pet.UserId == request.UserId && e.ParentVisitId == null)
                .OrderByDescending(e => e.Date)
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync(cancellationToken);

            var parentIds = parents.Select(p => p.Id).ToList();

            var rootParentMap = new Dictionary<Guid, Guid>();
            foreach (var p in parents) rootParentMap[p.Id] = p.Id;

            var allChildren = new List<HealthEvent>();
            var currentParentIds = parentIds;

            while (currentParentIds.Any())
            {
                var currentChildren = await _context.HealthEvents
                    .Include(e => e.EventType)
                    .Include(e => e.Pet)
                    .Include(e => e.Occurrences)
                    .Where(e => e.ParentVisitId != null && currentParentIds.Contains(e.ParentVisitId.Value))
                    .OrderBy(e => e.Date)
                    .ToListAsync(cancellationToken);

                if (!currentChildren.Any())
                    break;

                foreach (var child in currentChildren)
                {
                    rootParentMap[child.Id] = rootParentMap[child.ParentVisitId!.Value];
                }

                allChildren.AddRange(currentChildren);
                currentParentIds = currentChildren.Select(c => c.Id).ToList();
            }

            var allEvents = parents.Concat(allChildren).ToList();

            return allEvents.Select(e => MapToDto(e, rootParentMap));
        }

        private HealthEventDto MapToDto(HealthEvent e, Dictionary<Guid, Guid> rootParentMap)
        {
            var eventTypeDto = new EventTypeDefinitionDto
            {
                Id = e.EventType.Id,
                UserId = e.EventType.UserId,
                Name = e.EventType.Name,
                Icon = e.EventType.Icon,
                Color = e.EventType.Color
            };

            var parentId = e.ParentVisitId?.ToString();

            bool hasCompleted = e.Occurrences.Any(o => o.Status == OccurrenceStatus.Completed);

            var occurrencesDto = e.Occurrences.Select(o => new HealthEventOccurrenceDto
            {
                Id = o.Id,
                HealthEventId = o.HealthEventId,
                ScheduledDate = o.ScheduledDate,
                Status = o.Status.ToString(),
                CompletedAt = o.CompletedAt
            }).OrderBy(o => o.ScheduledDate).ToList();

            return e switch
            {
                VetVisitEvent v => new VetVisitEventDto { Id = v.Id, PetId = v.PetId, Date = v.Date, Title = v.Title, Notes = v.Notes, Weight = v.Weight, EventType = eventTypeDto, VeterinarianName = v.VeterinarianName, ClinicName = v.ClinicName, Diagnosis = v.Diagnosis, IsHospitalization = v.IsHospitalization, DischargeDate = v.DischargeDate, ParentId = parentId, HasCompletedOccurrences = hasCompleted, EndDate = v.EndDate, Occurrences = occurrencesDto },
                MedicationEvent m => new MedicationEventDto { Id = m.Id, PetId = m.PetId, Date = m.Date, Title = m.Title, Notes = m.Notes, Weight = m.Weight, EventType = eventTypeDto, DrugName = m.DrugName, Dosage = m.Dosage, FrequencyValue = m.FrequencyValue, FrequencyUnit = (int)m.FrequencyUnit, StartDate = m.StartDate, ParentId = parentId, HasCompletedOccurrences = hasCompleted, EndDate = m.EndDate, Occurrences = occurrencesDto },
                VaccineEvent v => new VaccineEventDto { Id = v.Id, PetId = v.PetId, Date = v.Date, Title = v.Title, Notes = v.Notes, Weight = v.Weight, EventType = eventTypeDto, VaccineName = v.VaccineName, FrequencyValue = v.FrequencyValue, FrequencyUnit = (int)v.FrequencyUnit, ParentId = parentId, HasCompletedOccurrences = hasCompleted, EndDate = v.EndDate, Occurrences = occurrencesDto },
                CustomHealthEvent c => new CustomHealthEventDto { Id = c.Id, PetId = c.PetId, Date = c.Date, Title = c.Title, Notes = c.Notes, Weight = c.Weight, EventType = eventTypeDto, FrequencyValue = c.FrequencyValue, FrequencyUnit = (int)c.FrequencyUnit, ParentId = parentId, HasCompletedOccurrences = hasCompleted, EndDate = c.EndDate, Occurrences = occurrencesDto },
                _ => throw new NotImplementedException()
            };
        }
    }
}
